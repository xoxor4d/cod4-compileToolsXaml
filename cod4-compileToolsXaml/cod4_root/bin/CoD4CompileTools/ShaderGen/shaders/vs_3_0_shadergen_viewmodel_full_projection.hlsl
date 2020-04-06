#define PC
#define IS_VERTEX_SHADER 1
#define IS_PIXEL_SHADER 0
#include <shader_vars.h>

struct VertexInput
{
	float4 position : POSITION;
    float4 normal   : NORMAL;
};

struct PixelInput
{
    float4 position     : POSITION;
    float3 worldPos     : TEXCOORD1;
    float3 worldNormal  : TEXCOORD2;
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
    pixel.position = mul( pixel.position, viewProjectionMatrix ); 

    // transform normal for current vertex
    float4 normal1;
        normal1 = float4( vertex.normal.xyz * c99.x, vertex.normal.w * c99.y) + c99.zzzw;

    float3 normal2;
        normal2.xyz = normal1.w * normal1.xyz;
    
    // use transformed vertex normal as weight for the projection blending
    pixel.worldNormal = normal2.xyz;
    
    // use vertex positions as texcoords in our ps
    pixel.worldPos = vertex.position.xyz; 
    
	return pixel;
}