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
matrix View;
matrix Projection;

// Toggle between Basic and PBR shading
bool usePBR;

// Camera
float3 viewPos;

// Directional Light
bool dirlightEnabled;
float3 dirlightDirection;
float4 dirlight_ambientColor;
float4 dirlight_diffuseColor;
float4 dirlight_specularColor;

// Point Light
bool pointlight1Enabled;
float3 pointlight1_position;
float4 pointlight1_ambientColor;
float4 pointlight1_diffuseColor;
float4 pointlight1_specularColor;
float pointlight1_constant;
float pointlight1_linear;
float pointlight1_quadratic;

// Spotlight
bool spotlight1Enabled;
float3 spotlight1_position;
float3 spotlight1_direction;
float4 spotlight1_ambientColor;
float4 spotlight1_diffuseColor;
float4 spotlight1_specularColor;
float spotlight1_constant;
float spotlight1_linear;
float spotlight1_quadratic;
float spotlight1_innerCutoff;   // in radians
float spotlight1_outerCutoff;   // in radians

//-------------------------------------
//         TEXTURE SAMPLERS           
//-------------------------------------

sampler2D texture_diffuse1 : register(s0);
sampler2D texture_normal1  : register(s1); // Normal map for PBR

//-------------------------------------
//           INPUT STRUCTURE           
//-------------------------------------

struct VertexShaderInput
{
    float4 aPos       : POSITION0;
    float3 aNormal    : NORMAL0;
    float2 aTexCoords : TEXCOORD0;
    float4 aTangent   : TANGENT0;    // tangent + sign for bitangent
};

//-------------------------------------
//          OUTPUT STRUCTURE          
//-------------------------------------

struct VertexShaderOutput
{
    float4 Position  : SV_POSITION;
    float3 FragPos   : TEXCOORD0;
    float2 TexCoords : TEXCOORD1;
    float3 NormalWS  : TEXCOORD2;
    float3x3 TBN     : TEXCOORD3;
};

//-------------------------------------
//           VERTEX SHADER            
//-------------------------------------

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;

    // World-space position
    float4 worldPos4 = mul(input.aPos, World);
    output.FragPos = worldPos4.xyz;
    output.Position = mul(mul(worldPos4, View), Projection);

    // Pass through texture coords
    output.TexCoords = input.aTexCoords;

    // Compute world-space normal
    output.NormalWS = normalize(mul(input.aNormal, (float3x3)World));

    // Construct TBN matrix for normal mapping
    float3 T = normalize(mul(input.aTangent.xyz, (float3x3)World));
    float3 B = cross(output.NormalWS, T) * input.aTangent.w;
    output.TBN = float3x3(T, B, output.NormalWS);

    return output;
}

//-------------------------------------
//           PIXEL SHADER            
//-------------------------------------

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    // Sample albedo
    float4 albedo = tex2D(texture_diffuse1, input.TexCoords);

    // Determine normal to use
    float3 N = input.NormalWS;
    if (usePBR)
    {
        // Sample normal map, transform from tangent to world space
        float3 normalMap = tex2D(texture_normal1, input.TexCoords).xyz * 2 - 1;
        N = normalize(mul(normalMap, input.TBN));
    }

    // Compute view direction
    float3 V = normalize(viewPos - input.FragPos);

    // Initialize accumulators
    float3 directionalLight = float3(0,0,0);
    float3 pointlight        = float3(0,0,0);
    float3 spotlight         = float3(0,0,0);

    // --- Directional Light ---
    if (dirlightEnabled)
    {
        float3 L = normalize(-dirlightDirection);
        float NdotL = max(dot(N, L), 0.0);
        float3 diffuse  = dirlight_diffuseColor.rgb * NdotL;
        float3 R = reflect(-L, N);
        float spec = pow(max(dot(V, R), 0.0), 32);
        float3 specular = dirlight_specularColor.rgb * spec;
        directionalLight = dirlight_ambientColor.rgb + diffuse + specular;
    }

    // --- Point Light ---
    if (pointlight1Enabled)
    {
        float3 L = normalize(pointlight1_position - input.FragPos);
        float NdotL = max(dot(N, L), 0.0);
        float3 diffuse = pointlight1_diffuseColor.rgb * NdotL;
        float3 R = reflect(-L, N);
        float spec = pow(max(dot(V, R), 0.0), 32);
        float3 specular = pointlight1_specularColor.rgb * spec;

        // Attenuation
        float distance = length(pointlight1_position - input.FragPos);
        float attenuation = 1.0 / (pointlight1_constant 
                                  + pointlight1_linear * distance 
                                  + pointlight1_quadratic * distance * distance);

        pointlight = (pointlight1_ambientColor.rgb + diffuse + specular) * attenuation;
    }

    // --- Spotlight ---
    if (spotlight1Enabled)
    {
        float3 L = normalize(spotlight1_position - input.FragPos);
        float theta = dot(L, normalize(-spotlight1_direction));
        float epsilon = spotlight1_innerCutoff - spotlight1_outerCutoff;
        float intensity = saturate((theta - spotlight1_outerCutoff) / epsilon);

        float NdotL = max(dot(N, L), 0.0);
        float3 diffuse  = spotlight1_diffuseColor.rgb * NdotL * intensity;
        float3 R = reflect(-L, N);
        float spec = pow(max(dot(V, R), 0.0), 32) * intensity;
        float3 specular = spotlight1_specularColor.rgb * spec;

        float distance = length(spotlight1_position - input.FragPos);
        float attenuation = 1.0 / (spotlight1_constant 
                                  + spotlight1_linear * distance 
                                  + spotlight1_quadratic * distance * distance);

        spotlight = (spotlight1_ambientColor.rgb + diffuse + specular) * attenuation;
    }

    // Combine all lights and modulate with albedo
    float3 finalColor = (directionalLight + pointlight + spotlight) * albedo.rgb;
    return float4(finalColor, albedo.a);
}

//-------------------------------------
//              TECHNIQUE            
//-------------------------------------

technique BasicColorDrawingWithLights
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader  = compile PS_SHADERMODEL MainPS();
    }
};