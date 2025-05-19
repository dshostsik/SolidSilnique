#if OPENGL
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif


TextureCube CubeTexture : register(t0); // The cube map bound to texture register t0
SamplerState CubeSampler : register(s1); // The sampler bound to sampler register s0

matrix View;
matrix Projection;

struct VertexShaderInput
{
    float4 Position : POSITION0; // Vertex position
         // Vertex normal
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION; // Transformed position
    float3 TexCord : TEXCOORD0;     // Pass normal to pixel shader
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

     
    output.Position = mul(input.Position, mul(View,Projection));

    
    output.TexCord = -input.Position;

    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    return CubeTexture.Sample(CubeSampler, normalize(float3(input.TexCord.x,input.TexCord.y*-1,input.TexCord.z)));
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};