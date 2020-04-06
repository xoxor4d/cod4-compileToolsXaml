#define PC
#define IS_VERTEX_SHADER 0
#define IS_PIXEL_SHADER 1
#include <shader_vars.h>

// General Projection Settings
#define PROJMAP_INTENSITY 2.0               // Projection Color Intensity
#define WORLDPOS_TO_PROJECTION_SCALE 32.0   // 1.0 / size * 32.0 = 32.0; => to somewhat match the size 
                                            // of shaders you can find on sizes like glslsandbox

// our input struct ( same as in vs obv. )
struct PixelInput
{
    float4 position     : POSITION;
    float3 worldPos     : TEXCOORD1;
};

// output struct
struct PixelOutput
{
	float4 color        : COLOR;
};

// sample function that draws tiling squares
// ++ remove me ++ //
float3 squares( float2 uvs, int colorMod = 0 )
{
    float2 f = floor(uvs);
    float2 m = fmod(f, 8.0);

    float3 finalColor;
    if(colorMod == 0) {
        finalColor = float3( m, 0.0 );
    }

    else if(colorMod == 1) {
        finalColor = float3( 0.0, m );
    }

    else {
        finalColor = float3( m.x, 0.0, m.y );
    }

    // only 1 return is allowed :(
    return finalColor;
}
// ++ remove me ++ //

// main ps entry, has to return the full output struct ( 1 float4 :: color r g b a )
PixelOutput ps_main( const PixelInput pixel )
{
    // define our output struct as "fragment"
    PixelOutput fragment;

    ///////////////////////
    // Main Shader Settings

    // Settings for Front, Side and Top Projections 
    // this shadertype only uses 1 sided projection ( SIDE )

    float   SIDE_SCALE      = 1.0;                  // 0.5 means half the size
    float2  SIDE_ASPECT     = float2( 1.0, 1.0 );   // 0.5 means more strech; 2.0 will compress
    float2  SIDE_OFFSET     = float2( 0.0, 0.0 );   // -x -> front; +y -> right

    // setup "UVs" with worldPositions for Side Projection
    float2 UV_SIDE = (pixel.worldPos.xz * (1.0 / SIDE_SCALE) * WORLDPOS_TO_PROJECTION_SCALE) * SIDE_ASPECT + SIDE_OFFSET;


    // most shader logic only needs UVs as input; main shader logic below this

    // predefine colors that are used in "projection blending" below
    // your shader has to place its final color output into these
    float3 COLOR_SIDE;

    // square function as sample
    // ++ remove me ++ //
    COLOR_SIDE  = squares(UV_SIDE, 0);
    // ++ remove me ++ //






    //////////////////////////////////
    // projection blending

    float4 tpSample;

    tpSample.xyz = COLOR_SIDE;
    tpSample.xyz *= tpSample.xyz * PROJMAP_INTENSITY;
    tpSample.w = 1.0f; // alpha is always 1

    // put the final color into our struct
    fragment.color = tpSample;

    // return full output struct
    return fragment;
}