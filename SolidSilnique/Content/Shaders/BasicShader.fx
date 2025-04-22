#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

matrix World;
matrix View;
matrix Projection;

struct VertexShaderInput {
    float3 aPos : POSITION0;
    float3 aNormal : NORMAL0;
    float2 aTexCoords : TEXCOORD0;
};

struct VertexShaderOutput {
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float2 TexCoords : TEXCOORD0;
};

VertexShaderOutput MainVS(VertexShaderInput input) {
    VertexShaderOutput output;
    output.Position = mul(float4(input.aPos, 1.0f), mul(mul(World, View), Projection));
    output.TexCoords = input.aTexCoords;
    output.Normal = input.aNormal;
    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET {
    return float4(input.Normal * 0.5 + 0.5, 1.0);
}

technique Basic {
    pass P0 {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};