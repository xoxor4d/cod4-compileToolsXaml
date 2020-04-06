#define PC
#define IS_VERTEX_SHADER 1
#define IS_PIXEL_SHADER 0
#include <shader_vars.h>

// input struct
struct VertexInput
{
	float4 position : POSITION;
};

// output struct
struct PixelInput
{
	float4 position : POSITION;
	float3 worldpos : TEXCOORD;
	float3 camerapos: TEXCOORD1;
};

// main vs entry, has to return the full output struct
PixelInput vs_main( const VertexInput vertex ) 
{
	// define our output struct as "pixel"
	PixelInput pixel;

	float4 dir = float4(vertex.position.xyz, 0.1);

	pixel.position = mul(dir, worldViewProjectionMatrix);
	pixel.worldpos = dir.xyz;
	pixel.camerapos = inverseWorldMatrix[3].xyz;

	return pixel;
}
