#define PC
#define IS_VERTEX_SHADER 0
#define IS_PIXEL_SHADER 1
#include <shader_vars.h>

// ++ remove me ++ //
#define ITER 32    		// amount of shapes - higher values will drop more frames

#define R_FACTOR 0.8
#define G_FACTOR 0.3
#define B_FACTOR 1.0
// ++ remove me ++ //

#define WORLDPOS_CENTER float3( 0.0, 0.0, -1.0 )		
	// The center of your shader is always the tag_origin of your weapon
	// add an offset here to move the center
	// +x will move the origin towards the camera
	// +y will acts as zoom? idk
	// +z will move it downwards

//#define WORLDPOS_CENTER	float3( 0.0, sunSpecular.x, glowApply.w ) 	
	// used for debug placement ingame (uncomment this later to enable it)
	// use "r_specularColorScale" for x offset ( 0 - 100 )
	// use "r_glowTweakBloomIntensity0" for z offset ( 0 - 20 ) ( needs "r_glowUseTweaks 1" )

#define WORLDPOS_TO_PROJECTION_SCALE 64.0   		
	// 1.0 / size * 32.0 = 32.0; => to somewhat match the size 
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

// http://glslsandbox.com/e#56795.0
// ++ remove me ++ //
float3 field(float3 p) 
{
	p *= 0.1;
	float f = 0.1;

	for (int i = 0; i < 5; i++) 
    {
		p = mul(p.yzx, float3x3( 0.8, 0.6, 0.0, 
                                -0.6, 0.8, 0.0, 
                                 0.0, 0.0, 1.0 ));
                                 
		p += float3( 0.123, 0.456, 0.789 ) * i;
		p = abs(frac(p) - 0.5);
		p *= 2.0;
		f *= 2.0;
	}

	p *= p;
	return sqrt( p + p.yzx ) / f - 0.002;
}
// ++ remove me ++ //

// main ps entry, has to return the full output struct ( 1 float4 :: color r g b a )
PixelOutput ps_main( const PixelInput pixel )
{
    // define our output struct as "fragment"
    PixelOutput fragment;

    // only UVs are needed for most shaders
    float3 UVs = normalize( (pixel.worldPos + WORLDPOS_CENTER) * WORLDPOS_TO_PROJECTION_SCALE );

	// ++ remove me ++ //
	float3 pos = float3( 0.4, 0.5, gameTime.w );
	float3 col = float3( 0.0, 0.0, 0.0 );

	for (int i = 0; i < ITER; i++) 
    {
		float3 f2 = field( pos );
		float f = min( min( f2.x, f2.y ), f2.z );
		
		pos += UVs * f;
		col += ( ITER - i ) / ( f2 + float3( 0.001, 0.001, 0.001) );
	}

	float3 color3 = float3( 1.0 - 1.0 / ( 1.0 + col * ( 0.09 / float( ITER * ITER ))));
	color3 *= color3; 

	fragment.color = float4( color3.r * R_FACTOR, color3.g * G_FACTOR, color3.b * B_FACTOR, 1.0 );
	// ++ remove me ++ //

	return fragment;
}