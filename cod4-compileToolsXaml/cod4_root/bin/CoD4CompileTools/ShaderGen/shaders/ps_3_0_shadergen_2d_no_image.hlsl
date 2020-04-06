#define PC
#define IS_VERTEX_SHADER 0
#define IS_PIXEL_SHADER 1
#include <shader_vars.h>

// our input struct ( same as in vs obv. )
struct PixelInput
{
    float4 position     : POSITION;
	float2 texcoord 	: TEXCOORD0;
};

// output struct
struct PixelOutput
{
	float4 color        : COLOR;
};

// main ps entry, has to return the full output struct ( 1 float4 :: color r g b a )
PixelOutput ps_main( const PixelInput pixel )
{
    // define our output struct as "fragment"
    PixelOutput fragment;

	fragment.color = float4( sin(gameTime.w), pixel.texcoord.x, pixel.texcoord.y, 1.0f );
	return fragment;
}