#define PC
#define IS_VERTEX_SHADER 0
#define IS_PIXEL_SHADER 1
#include <shader_vars.h>

#define OCT     4
#define ITER    30
#define EPS     0.001
#define NEAR    1.0
#define FAR     3.0

// input struct
struct PixelInput
{
    float4 position : POSITION;
	float3 worldpos : TEXCOORD0;
};

// output struct
struct PixelOutput
{
	float4 color    : COLOR;
};

float3 rotX(float3 p, float a) { return float3(p.x,                         p.y * cos(a) - p.z * sin(a), p.y * sin(a) + p.z * cos(a)); }
float3 rotY(float3 p, float a) { return float3(p.x * cos(a) - p.z * sin(a), p.y,                         p.x * sin(a) + p.z * cos(a)); }
float3 rotZ(float3 p, float a) { return float3(p.x * cos(a) - p.y * sin(a), p.x * sin(a) + p.y * cos(a), p.z); }

float3 hsv(float h, float s, float v) 
{ 
    return ((clamp(abs(frac(h + float3(0.0, 0.666, 0.333)) * 6.0 - 3.0) - 1.0, 0.0, 1.0) - 1.0) * s + 1.0) * v; 
}

float map(float3 p)
{
	float s = 0.98, df = 1.0;

	p = rotX(p, gameTime.w * 0.17);
    p = rotY(p, gameTime.w * 0.13);
    p += 1.0;

	for(int i = 0; i < OCT; i++)
    {
		if(p.x > 1.0) {
            p.x = 2.0 -p.x;
        }
        else if(p.x < -1.0) {
            p.x =- 2.0 - p.x;
        }
		if(p.y > 1.0) {
            p.y = 2.0 - p.y;
        }
        else if(p.y < -1.0) {
            p.y =- 2.0 - p.y;
        }
		if(p.z > 1.0) {
            p.z = 2.0 - p.z;
        }
        else if(p.z < -1.0) {
            p.z =- 2.0 - p.z;
        }

		float q = p.x * p.x + p.y * p.y + p.z * p.z;
		
        if(q < 0.25)
        {
            p  *= 4.0;
            df *= 3.0;
        }
        else if(q < 1.0)
        {
            p  *= 1.0 / q;
            df *= 0.9 / q;
        }

		p  *= s;
        p  += 0.2;
        df *= s;		
	}

	return (length(p) -1.55) / df;
}

float trace(float3 ro, float3 rd, out float n)
{
    float t = NEAR, d;
	for(int i = 0 ; i < ITER; i++)
    {
        d = map(ro + rd * t);

        if( abs(d) < EPS || t > FAR)
            i = ITER;

        t += step(d, 1.0) * d * 0.2 + d * 0.5;
        n += 1.0;
    }

	return min(t, FAR);
}

// main ps entry, has to return the full output struct ( 1 float4 :: color r g b a )
PixelOutput ps_main( const PixelInput pixel )
{
    // define our output struct as "fragment"
    PixelOutput fragment;

    float3 p = normalize(pixel.worldpos.xzy);
	float  n = 0.0, v = trace(float3( 0.0, -1.0, 0.0 ), p, n) * 0.3;
           n /= float( ITER );

    float3 textureSample = texCUBE(skyMapSampler, pixel.worldpos);

	fragment.color = float4( lerp( hsv( v + gameTime.w * 0.05, 0.5, n ), float3( 1.0, 1.0, 1.0 ), n ), 1.0 );

    // mix in your cubeMap 
	fragment.color.rgb = lerp(fragment.color.rgb, textureSample, 0.5);
	
	return fragment;
}
