#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

matrix LightViewProj;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float Depth : TEXCOORD0;
};

VertexShaderOutput MainVS(float4 Position : SV_POSITION)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(Position, LightViewProj);
	output.Depth = output.Position.xyz / output.Position.w;

	return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    return float4(input.Depth, 0, 0, 0);
}

technique ShadeTheSceneRightNow
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};