#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

// Technique
technique DrawLeafParticles
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_Particle();
        PixelShader = compile PS_SHADERMODEL PS_Leaf();
    }
};

// Buffers
cbuffer TimeBuffer : register(b0)
{
    float currentTime;
    float lifeTime;
    float3 gravity;
    float _padding; // for 16-byte alignment
}

cbuffer Matrices : register(b1)
{
    matrix View;
    matrix Projection;
}

//Texture2D LeafTexture : register(t0);
sampler2D LeafSampler;//: register(s0);

// Per-vertex input: 1 vertex = 1 corner of a quad
struct VS_IN
{
    float3 initialPos : POSITION0; // Particle base position
    float3 velocity : NORMAL0; // Velocity
    float spawnTime : TEXCOORD0; // Spawn time
    float2 corner : TEXCOORD1; // Per-vertex offset (-1,-1), (1,-1), etc. to build quad
};

struct VS_OUT
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
};

// Vertex Shader (billboard + particle motion)
VS_OUT VS_Particle(VS_IN input)
{
    VS_OUT o;

    float age = currentTime - input.spawnTime;
    if (age < 0 || age > lifeTime)
    {
        o.pos = float4(-9999, -9999, -9999, 1); // push offscreen
        o.uv = float2(0, 0);
        return o;
    }

    // Apply physics: p = p0 + vt + ½gt²
    float3 worldPos = input.initialPos
                    + input.velocity * age
                    + 0.5f * gravity * age * age;

    // Particle grows/shrinks
    float size = lerp(0.1, 0.3, age / lifeTime);

    // Get billboard axes from View matrix
    float3 right = float3(View._11, View._21, View._31);
    float3 up = float3(View._12, View._22, View._32);

    // Expand to quad
    float3 offset = (input.corner.x * right + input.corner.y * up) * size;
    float3 finalPos = worldPos + offset;

    o.pos = mul(float4(finalPos, 1.0), mul(View, Projection));
    o.uv = input.corner * 0.5 + 0.5; // map (-1,-1)...(1,1) to (0,0)...(1,1)

    return o;
}

// Pixel Shader
float4 PS_Leaf(VS_OUT input) : SV_Target
{
    return tex2D(LeafSampler, input.uv);
}
