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

float farPlane;
float nearPlane;

int useInstancingShadows;


struct VertexShaderInput
{
    float4 aPos : POSITION0;
    
    float4 instanceRow0 : TEXCOORD10;
    float4 instanceRow1 : TEXCOORD11;
    float4 instanceRow2 : TEXCOORD12;
    float4 instanceRow3 : TEXCOORD13;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
	float z : TEXCOORD0;
	float w : TEXCOORD1;
	
    float4 instanceRow0 : TEXCOORD10;
    float4 instanceRow1 : TEXCOORD11;
    float4 instanceRow2 : TEXCOORD12;
    float4 instanceRow3 : TEXCOORD13;
};



VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
	
    float4x4 newWorld = World;
    
    if (useInstancingShadows != 0)
    {
        float4x4 instanceWorld = float4x4(
            input.instanceRow0,
            input.instanceRow1,
            input.instanceRow2,
            input.instanceRow3
        );
        newWorld = instanceWorld;
    }

    output.Position = mul(input.aPos, mul(newWorld, LightViewProj));
	output.z = output.Position.z; 
	output.w = output.Position.w;

	return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    //float depth = saturate(input.Depth);
    //float4 pos = input.Position;
    float depth = input.z / input.w;
    depth = depth * 0.5f + 0.5f;
    //depth = saturate(depth); 
    return depth; //float4(depth, depth, depth, 1); // grayscale
}

technique ShadeTheSceneRightNow
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};