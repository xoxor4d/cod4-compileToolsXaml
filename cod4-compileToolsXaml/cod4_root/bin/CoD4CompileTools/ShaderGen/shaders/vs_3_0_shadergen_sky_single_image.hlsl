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
	float3 worldpos : TEXCOORD0;
};

// main vs entry, has to return the full output struct
PixelInput vs_main( const VertexInput vertex ) 
{
	// define our output struct as "pixel"
	PixelInput pixel;

	pixel.worldpos = vertex.position;
	pixel.position = mul(float4(vertex.position.xyz, 0.0f), worldViewProjectionMatrix);

	return output;
}
