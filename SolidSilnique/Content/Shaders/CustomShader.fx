#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif
//-------------------------------------
//              UNIFORMS            
//-------------------------------------
matrix World;
matrix View;
matrix Projection;

//-------------------------------------
//           INPUT STRUCTURE            
//-------------------------------------
struct VertexShaderInput
{
	float3 aPos : POSITION0;
	float3 aNormal : NORMAL0;
	float2 aTexCoords : TEXCOORD0;
};
//-------------------------------------
//           OUTPUT STRUCTURE            
//-------------------------------------
struct VertexShaderOutput
{
    float4 Position: SV_POSITION;
	float3 FragPos : TEXCOORD1;
	float3 Normal : NORMAL0;
    float2 TexCoords : TEXCOORD0;
};
//-------------------------------------
//            VERTEX SHADER            
//-------------------------------------
VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    output.FragPos = mul(float4(input.aPos, 1.0f), World).xyz;
    output.TexCoords = input.aTexCoords;
    // хуйня
    // output.Normal = float3x3(transpose(inverse(World))) * input.aNormal;
    output.Normal = float3(1.f, 1.f, 1.f);
    

	output.Position = mul(float4(input.aPos, 1.0f), mul(World, mul(View, Projection)));
	return output;
}
//-------------------------------------
//              UNIFORMS            
//-------------------------------------

// Stupid shit looks like doesn't support structures
    float4 dirlight_direction;

    float4 dirlight_ambientColor;
    float4 dirlight_diffuseColor;
    float4 dirlight_specularColor;

    float4 pointlight1_position;
    // linear in HLSL is a key word, so linear -> linearAttenuation
    float pointlight1_linearAttenuation;
    float pointlight1_quadraticAttenuation;
    float pointlight1_constant;
    
    float4 pointlight1_ambientColor;
    float4 pointlight1_diffuseColor;
    float4 pointlight1_specularColor;

    float4 spotlight1_position;
    float4 spotlight1_direction;
    
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
    // Zeroing output color in case something goes wrong;
    float4 OutFragColor = float4(0.1f, 0.1f, 0.1f, 0.1f);

    float4 textureVector = tex2D(texture_diffuse1, input.TexCoords);
    float3 norm = normalize(input.Normal);
    float3 viewDir = normalize(viewPos - input.FragPos);
    
    float3 directionalLight = float3(0.f, 0.f, 0.f);
    float dirLightFactor = dirlightEnabled ? 1.0 : 0.0;
    {
        float3 ambient = dirlight_ambientColor.rgb * textureVector.rgb;
        
        float3 lightDir = normalize(-dirlight_direction.xyz);
        float diff = max(dot(norm, lightDir), 0.0);
        float3 diffuse = dirlight_diffuseColor.rgb * diff * textureVector.rgb; 
        
        float3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        float3 specular = dirlight_specularColor.rgb * diffuse * textureVector.rgb;
        directionalLight = ambient + diffuse + specular;
    }
    
    float3 pointlight = float3(0.f, 0.f, 0.f);
    float pointLightFactor = pointlight1Enabled ? 1.0 : 0.0; 
    {
        float3 pointAmbient = float3(1.f, 1.f, 1.f);
        float3 pointDiffuse = float3(1.f, 1.f, 1.f);
        float3 pointSpecular = float3(1.f, 1.f, 1.f);
        
        pointAmbient = pointlight1_ambientColor.rgb * textureVector.rgb;
        
        float3 lightDirPointLight = normalize(pointlight1_position.xyz - input.FragPos.xyz);
        float pointDiff = max(dot(norm, lightDirPointLight), 0.0);
        pointDiffuse = pointlight1_diffuseColor.rgb * pointDiff * textureVector.rgb;
        
        float3 reflectDirPoint = reflect(-lightDirPointLight, norm);
        float specPoint = pow(max(dot(viewDir, reflectDirPoint), 0.0), 32);
        pointSpecular = pointlight1_specularColor.rgb * specPoint * textureVector.rgb;
        
        float distance = length(pointlight1_position.xyz - input.FragPos);
        float attenuation = 1.0f / (pointlight1_constant + pointlight1_linearAttenuation * distance + pointlight1_quadraticAttenuation * (distance * distance));
        
        pointAmbient *= attenuation;
        pointDiffuse *= attenuation;
        pointSpecular *= attenuation;
        
        pointlight = pointAmbient + pointDiffuse + pointSpecular; 
    }
    
    float3 spotlight = float3(0.f, 0.f, 0.f);
    float spotLightFactor = spotlight1Enabled ? 1.0 : 0.0; 
    {
        float3 spotAmbient = float3(1.f, 1.f, 1.f);
        float3 spotDiffuse = float3(1.f, 1.f, 1.f);
        float3 spotSpecular = float3(1.f, 1.f, 1.f);
        
        spotAmbient = spotlight1_ambientColor.rgb * textureVector.rgb;
        
        float3 lightDirSpot = normalize(spotlight1_position.xyz - input.FragPos);
        float spotDiff = max(dot(norm, lightDirSpot), 0.0);
        spotDiffuse = spotlight1_diffuseColor.rgb * spotDiff * textureVector.rgb;
        
        float3 reflectSpot = reflect(-lightDirSpot, norm);
        float specSpot = pow(max(dot(viewDir, reflectSpot), 0.0), 32);
        spotSpecular = spotlight1_specularColor.rgb * specSpot * textureVector.rgb;
        
        float theta = dot(lightDirSpot, normalize(-spotlight1_direction.xyz));
        float epsilon = spotlight1_innerCut - spotlight1_outerCut;
        float intensity = clamp((theta - spotlight1_outerCut) / epsilon, 0.f, 1.f);
        
        spotDiffuse *= intensity;
        spotSpecular *= intensity;
        
        float spotDistance = length(spotlight1_position.xyz - input.FragPos);
        float spotAttenuation = 1.0f / (spotlight1_constant + spotlight1_linearAttenuation * spotDistance + spotlight1_quadraticAttenuation * (spotDistance * spotDistance));
        
         spotAmbient *= spotAttenuation;
         spotDiffuse *= spotAttenuation;
         spotSpecular *= spotAttenuation;
         
         spotlight = spotAmbient + spotDiffuse + spotSpecular;
    }
    
    OutFragColor = float4(directionalLight + pointlight + spotlight, 1.0);
    
	return OutFragColor;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};