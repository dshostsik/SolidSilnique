// SimpleShape.fx
#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

//-------------------------------------
//   UNIFORMS
//-------------------------------------
// Standard matrices (not strictly used here, but MonoGame expects them)
matrix World;
matrix View;
matrix Projection;

// Circle parameters
float2 Center    = float2(0.5, 0.5);   // UV‐space center
float  Radius    = 0.3;                // Radius in UV units (0..1)
float4 FillColor = float4(1, 0, 0, 1);  // Red fill
float4 BgColor   = float4(0, 0, 0, 1);  // Black background

//-------------------------------------
//   VERTEX & PIXEL STRUCTS
//-------------------------------------
struct VertexInput
{
    float4 Position : POSITION0;   // NDC coordinates
    float2 TexCoord : TEXCOORD0;   // UV (0..1)
};

struct PixelInput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

//-------------------------------------
//   VERTEX SHADER
//-------------------------------------
PixelInput VS_Main(VertexInput vin)
{
    PixelInput vout;
    vout.Position = vin.Position;
    vout.TexCoord = vin.TexCoord;
    return vout;
}

//-------------------------------------
//   PIXEL SHADER
//-------------------------------------
float4 PS_Main(PixelInput pin) : SV_TARGET
{
    // Compute distance from UV to the circle center
    float2 uv = pin.TexCoord;
    float d = distance(uv, Center);

    // If inside radius, output FillColor; otherwise BgColor
    if (d <= Radius)
        return FillColor;
    else
        return BgColor;
}

//-------------------------------------
//   TECHNIQUE
//-------------------------------------
technique DrawShape
{
    pass P0
    {
        VertexShader   = compile VS_SHADERMODEL VS_Main();
        PixelShader    = compile PS_SHADERMODEL PS_Main();
    }
}
