// PBR.fx
// Simplified Metallic-Roughness PBR with normal map

sampler TextureSampler : register(s0);
sampler NormalSampler  : register(s1);

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 CameraPosition;
float3 LightDirection;
float4 LightColor;

struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float4 Tangent  : TANGENT0;
};

struct VS_OUTPUT
{
    float4 Position  : SV_POSITION;
    float2 TexCoord  : TEXCOORD0;
    float3 WorldPos  : TEXCOORD1;
    float3x3 TBN     : TEXCOORD2; // tangent-space basis
};

VS_OUTPUT VS_Main(VS_INPUT input)
{
    VS_OUTPUT o;
    // Transform position
    float4 worldPos = mul(input.Position, World);
    o.Position = mul(mul(input.Position, World), View);
    o.Position = mul(o.Position, Projection);
    o.WorldPos = worldPos.xyz;

    // Construct TBN matrix
    float3 N = normalize(mul(input.Normal, (float3x3)World));
    float3 T = normalize(mul(input.Tangent.xyz, (float3x3)World));
    float3 B = cross(N, T) * input.Tangent.w;
    o.TBN = float3x3(T, B, N);

    o.TexCoord = input.TexCoord;
    return o;
}

float4 PS_Main(VS_OUTPUT input) : SV_TARGET
{
    // Sample textures
    float4 albedo    = tex2D(TextureSampler, input.TexCoord);
    float3 normalMap = tex2D(NormalSampler,  input.TexCoord).xyz * 2 - 1;
    float3 N = normalize(mul(normalMap, input.TBN));

    // Simple diffuse + specular
    float3 L = normalize(-LightDirection);
    float3 V = normalize(CameraPosition - input.WorldPos);
    float3 H = normalize(L + V);

    float  NdotL = saturate(dot(N, L));
    float  NdotV = saturate(dot(N, V));
    float  NdotH = saturate(dot(N, H));
    float  VdotH = saturate(dot(V, H));

    // Lambertian diffuse
    float3 diffuse = albedo.rgb * NdotL;
    // Blinn-Phong specular (placeholder for Cook-Torrance)
    float  specPower = 16;
    float3 specular = LightColor.rgb * pow(NdotH, specPower);

    float3 color = diffuse + specular;
    return float4(color, albedo.a);
}

technique PBR
{
    pass P0
    {
        VertexShader = compile vs_4_0 VS_Main();
        PixelShader  = compile ps_4_0 PS_Main();
    }
}