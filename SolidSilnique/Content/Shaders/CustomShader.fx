#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

#define defaultLength 10

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

    //-------DIRECTIONAL-LIGHT-----
    int dirlightEnabled;

    float3 dirlight_direction;

    float4 dirlight_ambientColor;
    float4 dirlight_diffuseColor;
    float4 dirlight_specularColor;
    //-----------------------------
    
    //-----------POINT-LIGHT-ARRAY---------
    // use this to avoid useless computing of zeroes
    int realPointlightArrayLength;
    int pointlightEnabled[defaultLength];
    float3 pointlight_position[defaultLength];
    // linear in HLSL is a key word, so linear -> linearAttenuation
    float pointlight_linearAttenuation[defaultLength];
    float pointlight_quadraticAttenuation[defaultLength];
    float pointlight_constant[defaultLength];
    
    float4 pointlight_ambientColor[defaultLength];
    float4 pointlight_diffuseColor[defaultLength];
    float4 pointlight_specularColor[defaultLength];
    //-------------------------------------
    
    //----------SPOTLIGHT-ARRAY------------
    // use this to avoid useless computing of zeroes
    int realSpotlightArrayLength;

    int spotlightEnabled[defaultLength];

    float3 spotlight_position[defaultLength];
    float3 spotlight_direction[defaultLength];
    
    float spotlight_linearAttenuation[defaultLength];
    float spotlight_quadraticAttenuation[defaultLength];
    float spotlight_constant[defaultLength];

    float spotlight_innerCut[defaultLength];
    float spotlight_outerCut[defaultLength];
    
    float4 spotlight_ambientColor[defaultLength];
    float4 spotlight_diffuseColor[defaultLength];
    float4 spotlight_specularColor[defaultLength];
    //-------------------------------------

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
        
    float3 totalPointLight = float3(0.f, 0.f, 0.f);  
    for (int i = 0; i < realPointlightArrayLength; i++) {
        float3 pointlight = float3(0.f, 0.f, 0.f);
        if (pointlightEnabled[i] == true)
        {
            float3 pointAmbient = float3(1.f, 1.f, 1.f);
            float3 pointDiffuse = float3(1.f, 1.f, 1.f);
            float3 pointSpecular = float3(1.f, 1.f, 1.f);
            
            pointAmbient = pointlight_ambientColor[i].rgb;
            
            float3 lightDirPointLight = normalize(pointlight_position[i] - input.FragPos.xyz);
            float pointDiff = max(dot(norm, lightDirPointLight), 0.0);
            pointDiffuse = pointlight_diffuseColor[i].rgb * pointDiff;
            
            float3 reflectDirPoint = reflect(-lightDirPointLight, norm);
            float specPoint = pow(max(dot(viewDir, reflectDirPoint), 0.0), 32);
            pointSpecular = pointlight_specularColor[i].rgb * specPoint;
            
            float distance = length(pointlight_position[i] - input.FragPos);
            float attenuation = 1.0f / (pointlight_constant[i] + pointlight_linearAttenuation[i] * distance + pointlight_quadraticAttenuation[i] * (distance * distance));
            
            pointAmbient *= attenuation;
            pointDiffuse *= attenuation;
            pointSpecular *= attenuation;
            
            pointlight = pointAmbient + pointDiffuse + pointSpecular; 
        }
        totalPointLight += pointlight;
    }
    
    float3 totalSpotlight = float3(0.f, 0.f, 0.f);
    for (int i = 0; i < realSpotlightArrayLength; i++) {
        float3 spotlight = float3(0.f, 0.f, 0.f);
        if (spotlightEnabled[i] == true)
        {
            float3 spotAmbient = float3(1.f, 1.f, 1.f);
            float3 spotDiffuse = float3(1.f, 1.f, 1.f);
            float3 spotSpecular = float3(1.f, 1.f, 1.f);
            
            spotAmbient = spotlight_ambientColor[i].rgb;
            
            float3 lightDirSpot = normalize(spotlight_position[i] - input.FragPos);
            float spotDiff = max(dot(norm, lightDirSpot), 0.0f);
            spotDiffuse = spotlight_diffuseColor[i].rgb * spotDiff;
            
            float3 reflectSpot = reflect(-lightDirSpot, norm);
            float specSpot = pow(max(dot(viewDir, reflectSpot), 0.0f), 32);
            spotSpecular = spotlight_specularColor[i].rgb * specSpot;
            
            float theta = dot(lightDirSpot, normalize(-spotlight_direction[i]));
            float epsilon = spotlight_innerCut[i] - spotlight_outerCut[i];
            float intensity = clamp((theta - spotlight_outerCut[i]) / epsilon, 0.f, 1.f);
            
            spotDiffuse *= intensity;
            spotSpecular *= intensity;
            
            float spotDistance = length(spotlight_position[i] - input.FragPos);
            float spotAttenuation = 1.0f / (spotlight_constant[i] + spotlight_linearAttenuation[i] * spotDistance + spotlight_quadraticAttenuation[i] * (spotDistance * spotDistance));
            
             spotAmbient *= spotAttenuation;
             spotDiffuse *= spotAttenuation;
             spotSpecular *= spotAttenuation;
             
             spotlight = spotAmbient + spotDiffuse + spotSpecular;
        }
        totalSpotlight += spotlight;
    }    
    
	return float4(directionalLight + totalPointLight + totalSpotlight, 1.0) * textureVector;
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