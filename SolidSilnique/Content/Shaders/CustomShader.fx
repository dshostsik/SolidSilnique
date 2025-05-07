#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif
//-------------------------------------
//              UNIFORMS            
//-------------------------------------
matrix World;
matrix WorldTransInv;
matrix View;
matrix Projection;

//-------------------------------------
//           INPUT STRUCTURE            
//-------------------------------------
struct VertexShaderInput
{
	float4 aPos : POSITION0;
	float4 aNormal : NORMAL0;
	float2 aTexCoords : TEXCOORD0;
};
//-------------------------------------
//           OUTPUT STRUCTURE            
//-------------------------------------
struct VertexShaderOutput
{
    float4 Position: SV_POSITION;
	float4 Normal : TEXCOORD0;
    float2 TexCoords : TEXCOORD1;
    float3 FragPos : TEXCOORD2;
};

//-------------------------------------
//            VERTEX SHADER            
//-------------------------------------
VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
    
    output.Position = mul(input.aPos, mul(mul(World, View), Projection));
    output.TexCoords = input.aTexCoords;
    //output.Normal = float3x3(transpose(inverse(World))) * input.aNormal;
    //output.Normal = transpose(inverse(input.aNormal));
    output.Normal = mul(transpose(WorldTransInv), input.aNormal);
    output.FragPos = mul(input.aPos, World).xyz;
    return output;
}

//-------------------------------------
//              UNIFORMS            
//-------------------------------------

// Stupid shit looks like doesn't support structures
    float3 dirlight_direction;

    float4 dirlight_ambientColor;
    float4 dirlight_diffuseColor;
    float4 dirlight_specularColor;

    float3 pointlight1_position;
    // linear in HLSL is a key word, so linear -> linearAttenuation
    float pointlight1_linearAttenuation;
    float pointlight1_quadraticAttenuation;
    float pointlight1_constant;
    
    float4 pointlight1_ambientColor;
    float4 pointlight1_diffuseColor;
    float4 pointlight1_specularColor;

    float3 spotlight1_position;
    float3 spotlight1_direction;
    
    float spotlight1_linearAttenuation;
    float spotlight1_quadraticAttenuation;
    float spotlight1_constant;
    
    float spotlight1_innerCut;
    float spotlight1_outerCut;
    
    float4 spotlight1_ambientColor;
    float4 spotlight1_diffuseColor;
    float4 spotlight1_specularColor;

bool dirlightEnabled;
bool pointlight1Enabled;
bool spotlight1Enabled;

sampler2D texture_diffuse1;
float3 viewPos;


//-------------------------------------
//           PIXEL SHADER            
//-------------------------------------
float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    float4 textureVector = tex2D(texture_diffuse1, input.TexCoords);
    float3 norm = normalize(input.Normal.xyz); 
    float3 viewDir = normalize(viewPos - input.FragPos);
    
    float3 directionalLight = float3(0.f, 0.f, 0.f);
    if (dirlightEnabled == true)
    {
        float3 ambient = dirlight_ambientColor.rgb;
        
        float3 lightDir = normalize(-dirlight_direction);
        float diff = max(dot(norm, lightDir), 0.0);
        float3 diffuse = dirlight_diffuseColor.rgb * diff; 
        
        float3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        float3 specular = dirlight_specularColor.rgb * spec;
        directionalLight = ambient + diffuse + specular;
    }
        
    float3 pointlight = float3(0.f, 0.f, 0.f);
    if (pointlight1Enabled == true)
    {
        float3 pointAmbient = float3(1.f, 1.f, 1.f);
        float3 pointDiffuse = float3(1.f, 1.f, 1.f);
        float3 pointSpecular = float3(1.f, 1.f, 1.f);
        
        pointAmbient = pointlight1_ambientColor.rgb;
        
        float3 lightDirPointLight = normalize(pointlight1_position - input.FragPos.xyz);
        float pointDiff = max(dot(norm, lightDirPointLight), 0.0);
        pointDiffuse = pointlight1_diffuseColor.rgb * pointDiff;
        
        float3 reflectDirPoint = reflect(-lightDirPointLight, norm);
        float specPoint = pow(max(dot(viewDir, reflectDirPoint), 0.0), 32);
        pointSpecular = pointlight1_specularColor.rgb * specPoint;
        
        float distance = length(pointlight1_position - input.FragPos);
        float attenuation = 1.0f / (pointlight1_constant + pointlight1_linearAttenuation * distance + pointlight1_quadraticAttenuation * (distance * distance));
        
        pointAmbient *= attenuation;
        pointDiffuse *= attenuation;
        pointSpecular *= attenuation;
        
        pointlight = pointAmbient + pointDiffuse + pointSpecular; 
    }
    
    float3 spotlight = float3(0.f, 0.f, 0.f);
    if (spotlight1Enabled == true)
    {
        float3 spotAmbient = float3(1.f, 1.f, 1.f);
        float3 spotDiffuse = float3(1.f, 1.f, 1.f);
        float3 spotSpecular = float3(1.f, 1.f, 1.f);
        
        spotAmbient = spotlight1_ambientColor.rgb;
        
        float3 lightDirSpot = normalize(spotlight1_position - input.FragPos);
        float spotDiff = max(dot(norm, lightDirSpot), 0.0f);
        spotDiffuse = spotlight1_diffuseColor.rgb * spotDiff;
        
        float3 reflectSpot = reflect(-lightDirSpot, norm);
        float specSpot = pow(max(dot(viewDir, reflectSpot), 0.0f), 32);
        spotSpecular = spotlight1_specularColor.rgb * specSpot;
        
        float theta = dot(lightDirSpot, normalize(-spotlight1_direction));
        float epsilon = spotlight1_innerCut - spotlight1_outerCut;
        float intensity = clamp((theta - spotlight1_outerCut) / epsilon, 0.f, 1.f);
        
        spotDiffuse *= intensity;
        spotSpecular *= intensity;
        
        float spotDistance = length(spotlight1_position - input.FragPos);
        float spotAttenuation = 1.0f / (spotlight1_constant + spotlight1_linearAttenuation * spotDistance + spotlight1_quadraticAttenuation * (spotDistance * spotDistance));
        
         spotAmbient *= spotAttenuation;
         spotDiffuse *= spotAttenuation;
         spotSpecular *= spotAttenuation;
         
         spotlight = spotAmbient + spotDiffuse + spotSpecular;
    }
    
	return float4(directionalLight + pointlight + spotlight, 1.0) * textureVector;
}

//-------------------------------------
//           CEL SHADER          
//-------------------------------------
float4 CelShader(VertexShaderOutput input) : SV_TARGET
{
    float4 textureVector = tex2D(texture_diffuse1, input.TexCoords);
    float3 norm = normalize(input.Normal.xyz);
    float3 viewDir = normalize(viewPos - input.FragPos);
    
    float3 directionalLight = float3(0.f, 0.f, 0.f);
    if (dirlightEnabled == true)
    {
        
        float3 ambient = dirlight_ambientColor.rgb;
        
        float3 lightDir = normalize(-dirlight_direction);
        float diff = max(dot(norm, lightDir), 0.0);
        
        // Quantize diffuse into bands (cel shading)
        float shade;
        if (diff > 0.95)
            shade = 1.0;
        else if (diff > 0.5)
            shade = 0.7;
        else if (diff > 0.25)
            shade = 0.4;
        else
            shade = 0.1;
        
        
        float3 diffuse = dirlight_diffuseColor.rgb * shade;
        
        float3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        spec = step(0.98, spec); // binary highlight
        float3 specular = dirlight_specularColor.rgb * spec;
        
        directionalLight = ambient + diffuse + specular;
    }
        
    float3 pointlight = float3(0.f, 0.f, 0.f);
    
    return float4(directionalLight, 1.0) * textureVector;
}

VertexShaderOutput OutlineVS(in VertexShaderInput input){

    VertexShaderOutput output = (VertexShaderOutput)0;


    float inflate = -0.05f;

    float4 posInflated = input.aPos + input.aNormal * inflate;

    output.Position = mul(posInflated, mul(mul(World, View), Projection));
    output.TexCoords = input.aTexCoords;

    output.Normal = mul(transpose(WorldTransInv), input.aNormal);
    output.FragPos = mul(input.aPos, World).xyz;

    return output;


}

float4 OutlinePS(VertexShaderOutput input) : SV_TARGET
{
    
    return float4(0.0, 0.0, 0.0, 1.0);
}

technique BasicColorDrawingWithLights
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}

technique CelShadingOutline
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL OutlineVS();
        PixelShader = compile PS_SHADERMODEL OutlinePS();
    }
    };

technique CelShading
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL CelShader();
    }
};