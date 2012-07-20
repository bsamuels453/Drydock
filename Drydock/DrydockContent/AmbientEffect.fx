float4x4 World;
float4x4 View;
float4x4 Projection;
float4 AmbientColor = (1,1,1,1);
float AmbientIntensity = 1;
float4x4 WorldInverseTranspose;
float3 DiffuseLightDirection = float3(1, 1, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 0;

////constant textures
texture HullTexture;

sampler2D HullSamp = sampler_state {
    Texture = (HullTexture);
    MinFilter = Anisotropic;
    MagFilter = Anisotropic;
    AddressU = Wrap;
    AddressV = Wrap;
	MipFilter = Linear;
};

////////////////////////////////////////////
/////////////////VERTEX SHADER//////////////
////////////////////////////////////////////
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	float4x4 WorldInverseTranspose = transpose(World);
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Normal = mul(input.Normal, WorldInverseTranspose);
    output.TextureCoordinate = input.TextureCoordinate;

    return output;
}

////////////////////////////////////////////
/////////////////PIXEL SHADER///////////////
////////////////////////////////////////////
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 pixelColor = tex2D(HullSamp, input.TextureCoordinate);
	float4 ambientContribution = (pixelColor) *( AmbientColor * AmbientIntensity);
	float4 shadedColor = ambientContribution;
	shadedColor.a = 1;
	return saturate(shadedColor);
}

technique Standard
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}