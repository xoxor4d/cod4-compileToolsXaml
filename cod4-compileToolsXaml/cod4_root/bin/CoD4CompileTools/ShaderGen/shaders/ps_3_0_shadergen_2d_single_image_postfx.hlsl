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

float4 sincity(float4 colorInput)
{
        colorInput = pow(colorInput, .45f);

        float3 bwcolor = dot(colorInput.rgb, 1.0f.rrr) * 0.33333f;
        float weight = smoothstep(0.1f, 0.25f, colorInput.r - bwcolor);

        bwcolor = pow(bwcolor * 1.1f, 2.0f);
        float3 colorout = lerp(bwcolor, colorInput * float3(1.1f, 0.5f, 0.5f), weight);

        return pow(float4(colorout, 1.0f), 2.2f);
}

// main ps entry, has to return the full output struct ( 1 float4 :: color r g b a )
PixelOutput ps_main( const PixelInput pixel )
{
    // define our output struct as "fragment"
    PixelOutput fragment;

    float4 textureSample = tex2D(colorMapSampler, pixel.texcoord.xy);
	float4 frameBuffer = tex2D(colorMapPostSunSampler, pixel.texcoord.xy);

	fragment.color = lerp(sincity(float4(frameBuffer.rgb, 1.0f)), textureSample, 0.5f);

	return fragment;
}