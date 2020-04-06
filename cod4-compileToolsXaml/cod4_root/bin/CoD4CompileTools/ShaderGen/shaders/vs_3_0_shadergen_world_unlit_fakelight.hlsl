#define PC
#define IS_VERTEX_SHADER 1
#define IS_PIXEL_SHADER 0
#include <shader_vars.h>

struct VertexInput
{
	float4 position 	: POSITION;
	float4 normal 		: NORMAL;
	float4 color 		: COLOR;
	float2 texCoords 	: TEXCOORD0;
};

struct PixelInput
{
    float4 position 	: POSITION;
	float4 color 		: COLOR;
	float2 texCoords 	: TEXCOORD0;
	float4 worldNormal 	: TEXCOORD1;
	float3 worldViewPos	: TEXCOORD2;
	float3 worldPos		: TEXCOORD3;
};

PixelInput vs_main( const VertexInput vertex ) 
{
	PixelInput pixel;

	pixel.position = mul( float4( vertex.position.xyz, 1.0f ), worldMatrix );
    pixel.position = mul( pixel.position, viewProjectionMatrix ); 

	pixel.color = vertex.color;
	pixel.texCoords = vertex.texCoords;

	pixel.worldViewPos = inverseWorldMatrix[3];
	pixel.worldPos = vertex.position.xyz;

	float4 c99 = float4( 0.00787401572, 0.00392156886, -1, 0.752941191 );

	float4 normal1;
    normal1 = float4( vertex.normal.xyz * c99.x, vertex.normal.w * c99.y) + c99.zzzw;

    float3 normal2;
    normal2.xyz = normal1.w * normal1.xyz;

	pixel.worldNormal = float4(normal2.xyz, 1.0);

	return pixel;
}
