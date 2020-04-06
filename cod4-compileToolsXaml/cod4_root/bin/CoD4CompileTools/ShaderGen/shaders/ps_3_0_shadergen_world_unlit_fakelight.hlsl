#define PC
#define IS_VERTEX_SHADER 0
#define IS_PIXEL_SHADER 1
#include <shader_vars.h>

struct PixelInput
{
	float4 position 	: POSITION;
	float4 color 		: COLOR;
	float2 texCoords 	: TEXCOORD0;
	float4 worldNormal 	: TEXCOORD1;
	float3 worldViewPos	: TEXCOORD2;
	float3 worldPos		: TEXCOORD3;
};

struct PixelOutput
{
	float4 color : COLOR;
};

PixelOutput ps_main( const PixelInput pixel )
{
    PixelOutput fragment;

	float lightIntensity;
	float3 lightDir;
	float4 color, textureSample;

	// Worldspace normals and viewDirection
	float3 wsNormal = normalize(pixel.worldNormal.xyz);
	float3 wsViewDir = normalize(pixel.worldViewPos - pixel.worldPos);

	float3 ambient = float3(0.2, 0.2, 0.2);

	// colormap
	textureSample = tex2D(colorMapSampler, pixel.texCoords);
	
	// diffuse
	lightDir = sunPosition.xyz;
	float diff = max(dot(wsNormal, lightDir), 0.0);
	float3 diffuse = mul(diff, sunDiffuse.rgb);

	// Specular
	float3 reflectDir = reflect(-lightDir, wsNormal);
	float  spec 	= pow(max(dot(wsViewDir, reflectDir), 0.0), 32);
	float3 specular = (sunSpecular.x * 0.5f) * spec * sunDiffuse.rgb;  

	// Final color
	fragment.color = float4((ambient + diffuse + specular) * pixel.color.rgb * textureSample.rgb, pixel.color.a);

	return fragment;
}
