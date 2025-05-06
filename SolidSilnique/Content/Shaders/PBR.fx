// PBR.fx
// Simplified Metallic-Roughness PBR with normal map
#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

//-------------------------------------
// UNIFORMS
//-------------------------------------

matrix World;
matrix View;
matrix Projection;

// Toggle PBR
bool usePBR;
// Camera
float3 viewPos;

// Material properties
float shininess;

// Textures
texture texture_albedo  : register(t0);
texture texture_normal  : register(t1);

sampler sampler_albedo : register(s0)
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler sampler_normal : register(s1)
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

//-------------------------------------
// INPUT / OUTPUT STRUCTS
//-------------------------------------

struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 UV       : TEXCOORD0;
    float4 Tangent  : TANGENT0;
};

struct VS_OUTPUT
{
    float4 Position : SV_POSITION;
    float3 WorldPos : TEXCOORD0;
    float2 UV       : TEXCOORD1;
    float3 Normal   : TEXCOORD2;
    float3 T       : TEXCOORD3;
    float3 B       : TEXCOORD4;
};

//-------------------------------------
// VERTEX SHADER
//-------------------------------------
VS_OUTPUT VS_Main(VS_INPUT IN)
{
    VS_OUTPUT OUT;
    // Transform to world
    float4 worldPos = mul(IN.Position, World);
    OUT.WorldPos = worldPos.xyz;
    // Clip
    OUT.Position = mul(mul(worldPos, View), Projection);
    OUT.UV       = IN.UV;
    // Normals
    OUT.Normal = normalize(mul(IN.Normal, (float3x3)World));
    float3 T = normalize(mul(IN.Tangent.xyz, (float3x3)World));
    float3 B = cross(OUT.Normal, T) * IN.Tangent.w;
    OUT.T = T; OUT.B = B;
    return OUT;
}

//-------------------------------------
// PIXEL SHADER
//-------------------------------------
float4 PS_Main(VS_OUTPUT IN) : SV_TARGET
{
    // Albedo
    float4 albedo = tex2D(sampler_albedo, IN.UV);
    // Normal mapping
    float3 N = normalize(IN.Normal);
    if (usePBR)
    {
        float3 nm = tex2D(sampler_normal, IN.UV).xyz * 2 - 1;
        float3x3 TBN = float3x3(IN.T, IN.B, IN.Normal);
        N = normalize(mul(nm, TBN));
    }
    // View direction
    float3 V = normalize(viewPos - IN.WorldPos);
    // Lighting: simple directional for demo
    float3 L = normalize(-float3(0,1,0));
    float3 H = normalize(L + V);
    float NdotL = max(dot(N, L), 0);
    float NdotH = max(dot(N, H), 0);
    float3 diffuse  = albedo.rgb * NdotL;
    float3 specular = pow(NdotH, shininess) * float3(1,1,1);
    float3 color    = diffuse + specular;
    return float4(color, albedo.a);
}

technique PBRTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_Main();
        PixelShader  = compile PS_SHADERMODEL PS_Main();
    }
};
