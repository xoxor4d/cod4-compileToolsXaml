#define PC
#define IS_VERTEX_SHADER 0
#define IS_PIXEL_SHADER 1
#include <shader_vars.h>

#define ATMOSPHEREHEIGHT 4500 //3500
#define time -gameTime.w / 20.0 + 0.5
#define FOV 0.5
#define AURORORHEIGHT 25.0
#define AURORORBRIGHTNESS 3.5
#define sunpos float3(0.0, sin(time), cos(time))
#define CENTER float3(0.0, -63710.0, 0.0)
#define DEFAULTCAMERAHEIGHT 100 //500

// input struct
struct PixelInput
{
	float4 position 	: POSITION;
	float3 worldpos 	: TEXCOORD;
	float3 camerapos 	: TEXCOORD1;
};

// output struct
struct PixelOutput
{
	float4 color : COLOR;
};

float athmosHeight(float3 cam)
{
	float athmosHeight;

	// clip camera position
	if( cam.y > ATMOSPHEREHEIGHT ) {
		athmosHeight = cam.y;
	}
	else {
		athmosHeight = ATMOSPHEREHEIGHT;
	}

	return athmosHeight;
}

float dts(float3 camm,float3 poss,float3 center, float radius,float sig)
{
	float returnVal;
	float a = pow((dot(normalize(poss - camm), camm - center)), 2.0) -pow(length(camm - center), 2.0) + pow(radius, 2.0);
	
	if (a < 0.0) {
		returnVal = -1.0;
	}
	else {
		returnVal = -(dot(normalize(poss - camm), (camm - center))) + sig * sqrt(a);
	}
	
	return returnVal;
}

float random(in float3 _st) 
{
    return fract(sin(dot(_st.xyz, float3(12.9898, 78.233, 82.19))) * 43758.5453123);
}

float starnoise(in float3 _st) 
{
    float3 i = floor(_st);
    float3 f = fract(_st);

    // Four corners in 2D of a tile
    float starthreshhold = 0.998;
    float a = float(random(i) > starthreshhold);
    float b = float(random(i + float3(1.0, 0.0,0.0)) > starthreshhold);
    float c = float(random(i + float3(0.0, 1.0,0.0)) > starthreshhold);
    float d = float(random(i + float3(1.0, 1.0,0.0)) > starthreshhold);

    float e = float(random(i + float3(0.0, 0.0,1.0)) > starthreshhold);
    float g = float(random(i + float3(1.0, 0.0,1.0)) > starthreshhold);
    float h = float(random(i + float3(0.0, 1.0,1.0)) > starthreshhold);
    float j = float(random(i + float3(1.0, 1.0,1.0)) > starthreshhold);

    f = (1.0 - cos(f * 3.1415)) / 2.0;

    float a1 = lerp(a, b, f.x);
    float a2 = lerp(c, d, f.x);
    float a3 = lerp(e, g, f.x);
    float a4 = lerp(h, j, f.x);

    float a5 = lerp(a1, a2, f.y);
    float a6 = lerp(a3, a4, f.y);

    return lerp(a5, a6, f.z);
}

float noise (in float3 _st) 
{
    float3 i = floor(_st);
    float3 f = fract(_st);

    // Four corners in 2D of a tile
    float a = random(i);
    float b = random(i + float3(1.0, 0.0, 0.0));
    float c = random(i + float3(0.0, 1.0, 0.0));
    float d = random(i + float3(1.0, 1.0, 0.0));

    float e = random(i + float3(0.0, 0.0, 1.0));
    float g = random(i + float3(1.0, 0.0, 1.0));
    float h = random(i + float3(0.0, 1.0, 1.0));
    float j = random(i + float3(1.0, 1.0, 1.0));

    f = (1.0 - cos(f * 3.1415)) / 2.0;

    float a1 = lerp(a, b, f.x);
    float a2 = lerp(c, d, f.x);
    float a3 = lerp(e, g, f.x);
    float a4 = lerp(h, j, f.x);

    float a5 = lerp(a1, a2, f.y);
    float a6 = lerp(a3, a4, f.y);

    return lerp(a5, a6, f.z);
}

float fbm(in float3 _st) 
{
    float v = 0.0;
    float a = 0.5;
    float r = 1.0;
	float3 shift = float3(100.0, 22.5, 44.0);

    for (int i = 0; i < 3; ++i)
	{
        v += a * noise(_st);
        r += a;
        _st = shift + _st * 2.0;
        _st = (sin(r) * _st + cos(r) * _st);
        a *= 0.5;
    }

    return v;
}

float edis(float3 from,float3 to)
{
	float returnVal;

	float3 plac = CENTER;
	float rad = -CENTER.y - 6.0; // water radius / increase to lower

	float a = dts(from, to, plac, rad, -1.0);
	float b = dts(from, to, plac, rad, 1.0);

	if(a < 0.0 && b < 0.0) {
		returnVal -1.0;
	}
	else if(a < 0.0) {
		returnVal = b;
	}
	else if(b < 0.0) {
		returnVal = a;
	}
	else {
		returnVal = min(a,b);
	}

	return returnVal;
}

float sdis(float3 from,float3 to, float3 cam)
{
	float returnVal;

	float3 plac = CENTER;
	float rad = -CENTER.y + athmosHeight( cam );

	float a = dts(from, to, plac, rad, -1.0);
	float b = dts(from, to, plac, rad, 1.0);

	if( a < 0.0 && b < 0.0) {
		returnVal = -1.0;
	}
	else if(a < 0.0) {
		returnVal = b;
	}
	else if(b < 0.0) {
		returnVal = a;
	}
	else {
		returnVal = min(a,b);
	}

	return returnVal;
}

float3 outscatter(float r)
{
	float3 a = float3(1.02651, 1.02651, 1.02651);
	float3 b = float3(620.0, 540.0, 460.0) * 1.0;
	float3 os = (2.0 * 3.1415 * 4079.660796735571 * a * a * r) / (b * b * b * b);

	return 1.0 - os;
}

float3 scatter(float intensity,float3 lightdir,float3 from, float3 to, float3 cam)
{
	float3 l = lightdir;
	float  d = intensity;

	float r = distance(from, to);
	float c = acos(dot(normalize(to - from), normalize(l)));
	float3 a = float3(1.02651, 1.02651, 1.02651);
	float3 b = float3(620.0, 540.0, 460.0) * 1.0;

	float si = sdis(lerp(to, from, 0.5) + l * 0.5, lerp(to, from, 0.5) + l, cam);
	float ai = sdis(lerp(to, from, 0.99) + l * 0.5, lerp(to, from, 0.99) + l, cam);
	float fi = 0.5 * (si + ai);
	float mol = 215443.469003;

	float area = sin(c) * r * (si + ai) * 0.5;
	float3 is = d * 100.0 * r * (779.180801368 * a * a * (1.5 + 0.5 * cos(2.0 * c)) / (pow(b, float3(4.0, 4.0, 4.0))));

	return (is * outscatter(((si + ai) / 2.0) * 300.0));
}

float getcloud(in float3 a)
{
    float r = fbm(a / 20.0 + float3(time * 3.0, time * 2.0, 0.0));
    float th = 0.55;

	return max(1.0 - r / th, 0.0);
}

float3 getwatnorm(in float3 a)
{
	float3 waveoffset = float3(0.0, 0.0, time * 5.0);
	float3 dn = normalize(a - CENTER) * 0.03;
	float3 d = float3(0.01, 0.01, 0.0);

	return normalize(cross(a + d.xzz + dn * fbm(waveoffset + a + d.xzz) - a - dn * fbm(waveoffset + a), 
						   a + d.zzx + dn * fbm(waveoffset + a + d.zzx) - a - dn * fbm(waveoffset + a)));
}

float afbm(in float3 a)
{
	return fbm(a) * max(1.0 - a.y, 0.0);
}

float3 getaurora(in float3 a, float3 cam )
{
    float av = AURORORHEIGHT;
    float3 acc = float3(0.0, 0.0, 0.0);

    for(float i = 0.0; i < av; i++)
	{
        float3 z = a;
        z.xz = z.xz / ((float(i) / (50.0)) + 1.0);

		float r = fbm(z / 800.0 + float3(0.0, time, 0.0));
    	float th = sin(210.11);

       	acc += float(r > th && r < (th + 0.05 * sin(distance(a.xz, cam.xz) / 8000.0))) 
		       * float3(0.0025 * pow(max(1.0 - abs(i / av), 0.0), 2.0), 0.025 * pow(abs(i / av - 0.5) * 2.0, 1.0), 0.0125 * pow((i / av), 1.0 / 2.0)).xzy 
			   * AURORORBRIGHTNESS * i / av;
    }

    return acc;
}

// main ps entry, has to return the full output struct ( 1 float4 :: color r g b a )
PixelOutput ps_main( const PixelInput pixel )
//float4 ps_main( float3 worldPos : TEXCOORD, float3 cameraPos: TEXCOORD1 ) : COLOR
{
	// define our output struct as "fragment"
    PixelOutput fragment;

	float3 cam = pixel.camerapos.xzy;
	cam.zy += DEFAULTCAMERAHEIGHT;
	
	float3 uv = pixel.worldpos.xzy;
    
    float3 screen = float3(0.0,0.0,0.0);
    screen.x = FOV * uv.x * (5.0);
    screen.y = FOV * uv.y * (5.0);
    screen.z = FOV * uv.z * (5.0);
    
	// clip camera
    if(cam.y <= 0.0) {
       cam.y = 0.0;
    }

	//if(cam.y > ATMOSPHEREHEIGHT ) {
	//	ATMOSPHEREHEIGHT = cam.y;
	//}
    
    float3 pos = cam + screen;
	float3 virtualPos = cam;

	float3 directionToFragmentPosition = normalize(uv); // pos-cam
	float3 lightDir = sunpos;

	float distanceToGround = edis(cam, pos);
    float3 groundPosition = cam + directionToFragmentPosition * distanceToGround;

	float preReflectionDistance = 0.0;
    float3 preReflectionDirection = float3(0.1, 0.1, 0.1);

   	if(distanceToGround > 0.0)
	{
       preReflectionDistance = distanceToGround * 1.5;
       cam = groundPosition;

       preReflectionDirection = directionToFragmentPosition;
       directionToFragmentPosition = reflect(directionToFragmentPosition, getwatnorm(groundPosition / 25.0));
    }
    
   	float distanceToSky = sdis(cam, cam + directionToFragmentPosition, cam);
    float3 skyPosition = cam + directionToFragmentPosition * distanceToSky;
   
    virtualPos = skyPosition;
    float3 skyColor = scatter(50.0, lightDir, cam, virtualPos, cam) + scatter(50.0, lightDir, float3(0.0, 0.0, 0.0), preReflectionDirection, cam) * preReflectionDistance;

    float sunColor = pow(max(dot(directionToFragmentPosition, float3(lightDir)), 0.0), 200.0);
    skyColor += 2.0 * sunColor * max(outscatter(150.0 * (distance(cam, virtualPos) + preReflectionDistance)), 0.0);

    float3 skyDirection = normalize(virtualPos - CENTER);
    float2 cloudColor =  float2((pow(getcloud(skyPosition / 50.0), 3.0)), (pow(getcloud(skyPosition / 50.0 + lightDir * 3.0 * float3(0.0, 0.0, 1.0)), 2.0)));

 	float3 groundDirection = normalize(groundPosition - CENTER);
    float2 skyPolarCoords = float2(atan2(skyDirection.y, skyDirection.z), asin(skyDirection.x) + 3.1415 / 2.0);
    
	float star = (starnoise(virtualPos / 50.0));
    float3 starCol = float3(star, star, star);
    starCol += getaurora(virtualPos - 50.0, cam);

    float a = sdis(virtualPos + lightDir * 0.5, virtualPos + lightDir, cam);
    float b = edis(virtualPos + lightDir * 0.5, virtualPos + lightDir);
    float distanceToNight = float(a > b) * a;

    skyColor *= max(outscatter((distanceToNight) * 300.0), float3(0.0, 0.0, 0.0));
    skyColor = lerp(skyColor,float3(1.0, 1.0, 1.0), cloudColor.x * 1.0);
    skyColor = lerp(skyColor,float3(0.9, 0.9, 0.9), cloudColor.y * 1.0);

    float3 fragmentColor = lerp(clamp(skyColor, float3(0.0, 0.0, 0.0), float3(1.0, 1.0, 1.0)), starCol, 1.0 - max(outscatter((distanceToNight) * 300.0), float3(0.0, 0.0, 0.0)));

	// we still have to sample our sky texture or the material will fail to compile
	float3 nullsky = texCUBE(skyMapSampler, pixel.worldpos).xyz;
	nullsky *= 0.0001;

	// the sampled sky texture has to contribute to the final color output in some way
	fragment.color = float4(nullsky + fragmentColor, 1.0f);
	
	return fragment;
}
