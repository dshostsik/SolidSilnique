#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

matrix LightViewProj;
matrix World;


struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
	float z : TEXCOORD0;
	float w : TEXCOORD1;
};

VertexShaderOutput MainVS(float4 Position : POSITION)
{
	VertexShaderOutput output;

	output.Position = mul(Position, mul(World, LightViewProj));
	output.z = output.Position.z; 
	output.w = output.Position.w;

	return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    //float depth = saturate(input.Depth);
    //float4 pos = input.Position;
    float depth = input.z / input.w;
    depth = saturate(depth); 
    return float4(depth, depth, depth, 1); // grayscale
}

technique ShadeTheSceneRightNow
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};