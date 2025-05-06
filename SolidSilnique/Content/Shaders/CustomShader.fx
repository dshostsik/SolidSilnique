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
// Specular exponent
float shininess;
// Camera position in world space
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
float spotlight1_innerCutoff;
float spotlight1_outerCutoff;

//-------------------------------------
//         TEXTURE SAMPLERS           
//-------------------------------------

sampler2D texture_diffuse1 : register(s0)
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D texture_normal1  : register(s1)
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

//-------------------------------------
//           INPUT STRUCTURE         
//-------------------------------------

struct VertexShaderInput
{
    float4 aPos       : POSITION0;
    float3 aNormal    : NORMAL0;
    float2 aTexCoords : TEXCOORD0;
    float4 aTangent   : TANGENT0;
};

//-------------------------------------
//          OUTPUT STRUCTURE         
//-------------------------------------

struct VertexShaderOutput
{
    float4 Position    : SV_POSITION;
    float3 FragPos     : TEXCOORD0;
    float2 TexCoords   : TEXCOORD1;
    float3 NormalWS    : TEXCOORD2;
    float3 TangentWS   : TEXCOORD3;
    float3 BitangentWS : TEXCOORD4;
};

//-------------------------------------
//           VERTEX SHADER           
//-------------------------------------

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;

    // Transform position
    float4 worldPos4 = mul(input.aPos, World);
    output.FragPos = worldPos4.xyz;
    output.Position = mul(mul(worldPos4, View), Projection);

    // Pass texture coordinates
    output.TexCoords = input.aTexCoords;

    // Compute normals and tangents in world space
    float3 normalWS = normalize(mul(input.aNormal, (float3x3)World));
    float3 tangentWS = normalize(mul(input.aTangent.xyz, (float3x3)World));
    float3 bitangentWS = cross(normalWS, tangentWS) * input.aTangent.w;

    output.NormalWS = normalWS;
    output.TangentWS = tangentWS;
    output.BitangentWS = normalize(bitangentWS);

    return output;
}

//-------------------------------------
//           PIXEL SHADER            
//-------------------------------------

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    // Albedo
    float4 albedo = tex2D(texture_diffuse1, input.TexCoords);

    // Normal mapping
    float3 N = input.NormalWS;
    if (usePBR)
    {
        float3 nm = tex2D(texture_normal1, input.TexCoords).xyz * 2 - 1;
        float3x3 TBN = float3x3(input.TangentWS, input.BitangentWS, input.NormalWS);
        N = normalize(mul(nm, TBN));
    }

    // View direction
    float3 V = normalize(viewPos - input.FragPos);

    // Initialize light sums
    float3 result = float3(0,0,0);

    // Directional Light
    if (dirlightEnabled)
    {
        float3 L = normalize(-dirlightDirection);
        float diff = max(dot(N, L), 0.0);
        float3 D = dirlight_diffuseColor.rgb * diff;
        float3 R = reflect(-L, N);
        float spec = pow(max(dot(V, R), 0.0), shininess);
        float3 S = dirlight_specularColor.rgb * spec;
        result += dirlight_ambientColor.rgb + D + S;
    }

    // Point Light
    if (pointlight1Enabled)
    {
        float3 L = normalize(pointlight1_position - input.FragPos);
        float diff = max(dot(N, L), 0.0);
        float3 D = pointlight1_diffuseColor.rgb * diff;
        float3 R = reflect(-L, N);
        float spec = pow(max(dot(V, R), 0.0), shininess);
        float3 S = pointlight1_specularColor.rgb * spec;
        float dist = length(pointlight1_position - input.FragPos);
        float att = 1.0 / (pointlight1_constant + pointlight1_linear*dist + pointlight1_quadratic*dist*dist);
        result += (pointlight1_ambientColor.rgb + D + S) * att;
    }

    // Spotlight
    if (spotlight1Enabled)
    {
        float3 L = normalize(spotlight1_position - input.FragPos);
        float theta = dot(L, normalize(-spotlight1_direction));
        float epsilon = spotlight1_innerCutoff - spotlight1_outerCutoff;
        float intensity = saturate((theta - spotlight1_outerCutoff) / epsilon);
        float diff = max(dot(N, L), 0.0);
        float3 D = spotlight1_diffuseColor.rgb * diff * intensity;
        float3 R = reflect(-L, N);
        float spec = pow(max(dot(V, R), 0.0), shininess) * intensity;
        float3 S = spotlight1_specularColor.rgb * spec;
        float dist = length(spotlight1_position - input.FragPos);
        float att = 1.0 / (spotlight1_constant + spotlight1_linear*dist + spotlight1_quadratic*dist*dist);
        result += (spotlight1_ambientColor.rgb + D + S) * att;
    }

    // Final color
    float3 finalCol = result * albedo.rgb;
    return float4(finalCol, albedo.a);
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