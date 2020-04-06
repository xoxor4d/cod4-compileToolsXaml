#define PC
#define IS_VERTEX_SHADER 0
#define IS_PIXEL_SHADER 1
#include <shader_vars.h>

// General Projection Settings
#define PROJMAP_BLENDOFFSET     0.25     // values higher then 1.0 will start to clip parts of the shader depending on the vertex normal
#define PROJMAP_BLENDEXPONENT   1.0      // default : 1.5; higher values will blend less
#define PROJMAP_INTENSITY       0.5      // Projection Color Intensity

#define WORLDPOS_TO_PROJECTION_SCALE 32.0   // 1.0 / size * 32.0 = 32.0; => to somewhat match the size 
                                            // of shaders you can find on sites like glslsandbox

// our input struct ( same as in vs obv. )
struct PixelInput
{
    float4 position     : POSITION;
    float3 worldPos     : TEXCOORD1;
    float3 worldNormal  : TEXCOORD2;
};

// output struct
struct PixelOutput
{
	float4 color        : COLOR;
};

// sample function that draws tiling squares
float3 squares( float2 uvs, int colorMod = 0 )
{
    float2 f = floor(uvs);
    float2 m = fmod(f, 8.0);

    float3 finalColor;
    if(colorMod == 0) 
    {
        finalColor = float3( m, 0.0 );
    }
    else if(colorMod == 1) 
    {
        finalColor = float3( 0.0, m );
    }
    else 
    {
        finalColor = float3( m.x, 0.0, m.y );
    }

    // only 1 return is allowed
    return finalColor;
}

// main ps entry, has to return the full output struct ( 1 float4 :: color r g b a )
PixelOutput ps_main( const PixelInput pixel )
{
    // define our output struct as "fragment"
    PixelOutput fragment;

    // *
    // main Shader Settings

    // Settings for Front, Side and Top Projections 
    // this shadertype uses 3 sided projection

    float   FRONT_SCALE     = 6.0;                  // 0.5 means half the size
    float2  FRONT_ASPECT    = float2( 1.0, 1.0 );   // 0.5 means more strech; 2.0 will compress
    float2  FRONT_OFFSET    = float2( 0.0, 0.0 );   // -x -> right; +y -> height ( from camera )

    float   SIDE_SCALE      = 6.0;                  // 0.5 means half the size
    float2  SIDE_ASPECT     = float2( 1.0, 1.0 );   // 0.5 means more strech; 2.0 will compress
    float2  SIDE_OFFSET     = float2( 0.0, 0.0 );   // -x -> front; +y -> right

    float   TOP_SCALE       = 6.0;                  // 0.5 means half the size
    float2  TOP_ASPECT      = float2( 1.0, 1.0 );   // 0.5 means more strech; 2.0 will compress
    float2  TOP_OFFSET      = float2( 0.0, 0.0 );   // -x -> front; +y -> right

    // *
    // we use untransformed coordinates (model-space) as our UV's
    float2 UV_FRONT = (pixel.worldPos.yz * (1.0 / FRONT_SCALE) * WORLDPOS_TO_PROJECTION_SCALE) * FRONT_ASPECT + FRONT_OFFSET;
    float2 UV_SIDE  = (pixel.worldPos.xz * (1.0 / SIDE_SCALE)  * WORLDPOS_TO_PROJECTION_SCALE) * SIDE_ASPECT  + SIDE_OFFSET;
    float2 UV_TOP   = (pixel.worldPos.xy * (1.0 / TOP_SCALE)   * WORLDPOS_TO_PROJECTION_SCALE) * TOP_ASPECT   + TOP_OFFSET;
    

    // *
    // main shader logic

    // predefine colors that are used in "projection blending" below
    // your shader has to place its final color output into these
    float3 COLOR_FRONT;
    float3 COLOR_SIDE;
    float3 COLOR_TOP;

    // square function as sample
    COLOR_FRONT = squares(UV_FRONT, 0);
    COLOR_SIDE  = squares(UV_SIDE, 1);
    COLOR_TOP   = squares(UV_TOP, 2);


    // *
    // projection blending

    float3 tpBlend;
    float4 tpSample;

	tpBlend  = abs( pixel.worldNormal );
	tpBlend  = saturate(tpBlend - PROJMAP_BLENDOFFSET);
	tpBlend  = pow(tpBlend, PROJMAP_BLENDEXPONENT);
	tpBlend /= (tpBlend.x + tpBlend.y + tpBlend.z);

    // Blend all 3 sides here
    
    tpSample.xyz = tpBlend.x * COLOR_FRONT;           // Front
    tpSample.xyz = tpBlend.y * COLOR_SIDE + tpSample; // Side
    tpSample.xyz = tpBlend.z * COLOR_TOP  + tpSample; // Top
    
    tpSample.xyz *= tpSample.xyz * PROJMAP_INTENSITY;
    tpSample.w = 1.0f; // alpha is always 1

    // put the final color into our struct
    fragment.color = tpSample;

    // return full output struct
    return fragment;
}
