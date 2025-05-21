//-----------------------------------------------------
// LeafParticle.fx
//-----------------------------------------------------
technique11 DrawLeafParticles
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, VS_Particle()));
        SetGeometryShader(CompileShader(gs_5_0, GS_ParticleToQuad()));
        SetPixelShader(CompileShader(ps_5_0, PS_Leaf()));
    }
}

cbuffer TimeBuffer : register(b0)
{
    float currentTime; // in seconds
    float lifeTime; // e.g. 5.0
    float3 gravity; // e.g. (0,-9.8,0)
    float _padding; // 16-byte align
}

cbuffer Matrices : register(b1)
{
    matrix View;
    matrix Projection;
}

Texture2D LeafTexture : register(t0);
SamplerState LeafSampler : register(s0);

// per-particle data
struct VS_IN
{
    float3 initialPos : POSITION0;
    float3 velocity : NORMAL0; // misuse NORMAL0 as a free semantic
    float spawnTime : TEXCOORD0; // when this leaf began its fall
};

struct GS_OUT
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
};

VS_IN particles[1]; // for geometry shader input

struct VS_OUT
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
};

VS_OUT
VS_Particle(VS_IN input)
{
    VS_OUT o;
    // simply pass through; GS will do the heavy lifting
    o.pos = float4(input.initialPos, 1);
    o.uv = float2(0, 0);
    return o;
}

[maxvertexcount(4)]
void
GS_ParticleToQuad(point VS_IN IN[1], inout TriangleStream<GS_OUT> Stream)
{
    float age = currentTime - IN[0].spawnTime;
    if (age < 0 || age > lifeTime)
        return;

    // basic physics: p = p0 + v t + ½ g t²
    float3 worldPos = IN[0].initialPos
                      + IN[0].velocity * age
                      + 0.5f * gravity * age * age;

    // Build a camera-facing quad around worldPos
    // We'll generate a unit quad in NDC, then scale + translate
    float size = lerp(0.1, 0.3, age / lifeTime); // leaves drift/grow a bit

    // Corners in object-space of quad:
    float3 corners[4] =
    {
        float3(-size, size, 0),
        float3(size, size, 0),
        float3(-size, -size, 0),
        float3(size, -size, 0),
    };
    float2 uvs[4] =
    {
        float2(0, 0),
        float2(1, 0),
        float2(0, 1),
        float2(1, 1),
    };

    // billboard: extract camera right/up from View
    float3 camRight = float3(View._11, View._21, View._31);
    float3 camUp = float3(View._12, View._22, View._32);

    for (int i = 0; i < 4; ++i)
    {
        float3 worldCorner = worldPos
                             + corners[i].x * camRight
                             + corners[i].y * camUp;
        GS_OUT o;
        o.pos = mul(float4(worldCorner, 1), View * Projection);
        o.uv = uvs[i];
        Stream.Append(o);
    }
    Stream.RestartStrip();
}

float4
PS_Leaf(GS_OUT input) : SV_Target
{
    return LeafTexture.Sample(LeafSampler, input.uv);
}
