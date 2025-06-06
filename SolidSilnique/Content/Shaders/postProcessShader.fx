// postProcessShader.fx

//-------------------------------------
// Semantic macros for XNA/MonoGame
//-------------------------------------
#if OPENGL
#define SV_POSITION    POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define SV_POSITION    POSITION
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//-------------------------------------
//   UNIFORMS (Parameters from C#)
//-------------------------------------
Texture2D PrevRendered;
SamplerState PrevRenderedSampler;

// A simple tint (set to float4(1,1,1,1) from C# if you don’t want any change)
float4 BrightnessTint;

//-------------------------------------
//   SPRITEBATCH‐COMPATIBLE VS INPUT/OUTPUT
//-------------------------------------
struct VS_INPUT
{
    float4 Position : POSITION0; // Already pre‐transformed to clip‐space
    float4 Color : COLOR0; // Vertex color / tint
    float2 TexCoord : TEXCOORD0; // 0..1
};

struct VS_OUTPUT
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

//-------------------------------------
//   VERTEX SHADER (passthrough for SpriteBatch)
//-------------------------------------
VS_OUTPUT VS_Main(VS_INPUT input)
{
    VS_OUTPUT output;
    // SpriteBatch already gives us clip‐space positions, so just pass through:
    output.Position = input.Position;
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;
    return output;
}

//-------------------------------------
//   PIXEL SHADER (just copies the texture for now)
//-------------------------------------
float4 PS_Main(VS_OUTPUT input) : SV_TARGET
{
    // Sample the “PrevRendered” render‐target
    float4 col = PrevRendered.Sample(PrevRenderedSampler, input.TexCoord);

    // Multiply by a tint (use this later when you do bloom‐brightening)
    col.rgb *= BrightnessTint.rgb;
    return col;
}

//-------------------------------------
//   TECHNIQUE FOR SPRITEBATCH
//-------------------------------------
technique PostProcess
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_Main();
        PixelShader = compile PS_SHADERMODEL PS_Main();
    }
}
