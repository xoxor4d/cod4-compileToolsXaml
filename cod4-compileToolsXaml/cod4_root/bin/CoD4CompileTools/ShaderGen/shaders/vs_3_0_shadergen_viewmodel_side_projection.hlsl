#define PC
#define IS_VERTEX_SHADER 1
#define IS_PIXEL_SHADER 0
#include <shader_vars.h>

struct VertexInput
{
	float4 position : POSITION;
};

struct PixelInput
{
    float4 position     : POSITION;
    float3 worldPos     : TEXCOORD1;
};

PixelInput vs_main( const VertexInput vertex ) 
{
	PixelInput pixel;
  
    // recreated default viewmodelshader to setup position, texcoords and normals
    // still messy but works

    float  shadowmapConst = 0.0009765625;
    float  oneQuarter     = 0.25;
    float  chunks         = 0.0078125; // 8.0 / 1024
    float  maxFLT         = 3.05175781e-005;
    float  noise          = 0.03125;
    float4 c99            = float4( 0.00787401572, 0.00392156886, -1, 0.752941191 );

    // ############################ POSITION BEGIN ###################################

    // transform the model from modelSpace to projectionSpace
    pixel.position = mul( float4( vertex.position.xyz, 1.0f ), worldMatrix );
    //pixel.worldPos = pixel.position.xyz; // doing it like this will project it depending on viewpos, because we wont have a fixed axis
    pixel.position = mul( pixel.position, viewProjectionMatrix ); 

    // use vertex positions as texcoords in our ps
    pixel.worldPos = vertex.position.xyz; 
    
	return pixel;
}