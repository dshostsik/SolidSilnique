
//-------------------------------------
// Semantic macros for XNA/MonoGame
//-------------------------------------
#if OPENGL
#define SV_POSITION    POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define SV_POSITION    POSITION
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

//-------------------------------------
//   UNIFORMS (Parameters from C#)
//-------------------------------------
Texture2D PrevRendered;
sampler PrevRenderedSampler;
Texture2D PrevRenderedColor;
sampler PrevRenderedSamplerColor;
// A simple tint (set to float4(1,1,1,1) from C# if you don't want any change)

struct VS_OUTPUT
{
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 LE_Blur(VS_OUTPUT input, float4 pixel)
{
    float2 texOffset;
    texOffset.x = 1.0 / 1920.0;
    texOffset.y = 1.0 / 1080.0;
    
    // Fixed array initialization
    float weight[10];
    weight[0] = 0.198596;
    weight[1] = 0.175713;
    weight[2] = 0.121788;
    weight[3] = 0.066052;
    weight[4] = 0.028074;
    weight[5] = 0.009336;
    weight[6] = 0.002432;
    weight[7] = 0.000496;
    weight[8] = 0.000079;
    weight[9] = 0.000010;
    
    // Initialize result with the center pixel
    float3 result = pixel.rgb * weight[0];
    float totalWeight = weight[0];
    
    
        for (int i = 0; i < 10; i++)
        {
        // Horizontal blur
            float2 offsetH = float2(texOffset.x * i, 0.0);
            result += PrevRendered.Sample(PrevRenderedSampler, input.TexCoord + offsetH).rgb * weight[i];
            result += PrevRendered.Sample(PrevRenderedSampler, input.TexCoord - offsetH).rgb * weight[i];
        
        // Vertical blur
            float2 offsetV = float2(0.0, texOffset.y * i);
            result += PrevRendered.Sample(PrevRenderedSampler, input.TexCoord + offsetV).rgb * weight[i];
            result += PrevRendered.Sample(PrevRenderedSampler, input.TexCoord - offsetV).rgb * weight[i];
        
            totalWeight += weight[i] * 4.0;
        }
    
        
    
    // Normalize the result
    result /= totalWeight;
    
    return float4(result, pixel.a);
}

//-------------------------------------
//   PIXEL SHADER
//-------------------------------------
float4 PS_Main(VS_OUTPUT input) : SV_TARGET
{
    float4 pixelColor = PrevRendered.Sample(PrevRenderedSampler, input.TexCoord);
    float4 resultPixel;
    
    float pixelAVG = (pixelColor.r + pixelColor.g + pixelColor.b) / 3.0;
    
    
    if (pixelAVG >= 0.7)
        {
            resultPixel = pixelColor;
            
        }
        else
        {
            resultPixel = float4(0, 0, 0, 1);
        }
        
        
        return resultPixel;
}

float4 Blooming(VS_OUTPUT input) : SV_TARGET
{
    float4 pixelColor = PrevRendered.Sample(PrevRenderedSampler, input.TexCoord);
    float4 resultPixel;
    
    float pixelAVG = (pixelColor.r + pixelColor.g + pixelColor.b) / 3.0;
    
    pixelColor = LE_Blur(input,pixelColor);
    
    resultPixel = lerp(pixelColor,PrevRenderedColor.Sample(PrevRenderedSamplerColor, input.TexCoord),0.5); 
        
        return resultPixel;
}

//-------------------------------------
//   TECHNIQUE FOR SPRITEBATCH
//-------------------------------------
technique PostProcess
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL PS_Main();
    }
}
technique Bloom
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL Blooming();
    }
}