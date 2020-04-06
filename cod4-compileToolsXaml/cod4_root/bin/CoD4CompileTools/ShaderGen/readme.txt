┐ ┌┌─┐┐ ┌┌─┐┬─┐┌┐┌┬┐ ┌─┐┬┌┬┐┬ ┬┬ ┬┌┐  ┬┌─┐
│x││ ││x││ │├┬┘└├ ││ │ ┬│ │ ├─┤│ │├┴┐ ││ │
┘ └└─┘┘ └└─┘┴└─ ┴ ┴┘o└─┘┴ ┴ ┴ ┴└─┘└─┘o┴└─┘
			  - https://xoxor4d.github.io/
			  
// *
// *
// ShaderGen Types:

[2D]
|-> Shader for HUD elements or fx materials, anything 2D really.
|-> Only produces non-prefixed techsets.

[VIEWMODEL]
|-> Used for viewmodels/viewhands. (Viewmodels are using transformed UVs, unlike normal models)
|-> Shadergen can create a modified material & xmodel (stock files only) that uses the generated shader.
|-> Only produces non-prefixed & mc_ techsets.

[XMODEL]
|-> Used for all xmodels except for viewmodels/viewhands.
|-> Only produces non-prefixed & mc_ techsets.

[WORLD]
|-> Used for all kinds of world materials. (e.g. materials you apply to brushes in the world)
|-> Currently supports: Unlit only.
|-> Only produces non-prefixed & wc_ techsets.

[SKY]
|-> Shader for skies.
|-> Only produces non-prefixed & wc_ techsets.


// *
// *
// * ShaderGen Options:

[2D]
|--> [NO_IMAGE]
| :: 2d shader that does not use any input images.
|
|--> [NO_IMAGE_POSTFX]
| :: 2d shader that uses the resampled scene as an input image.
| :: to be used for post fx shaders.
|
|--> [SINGLE_IMAGE]
| :: 2d shader that is only using the colorMap.
|
|--> [SINGLE_IMAGE_POSTFX]
| :: 2d shader that uses the colorMap and the resampled scene as input images.
| :: to be used for post fx shaders.


[VIEWMODEL]
|--> [FULL_PROJECTION]
| :: 2 pass shader. First pass shades the weapon using stock shaders (needs atleast a colorMap), second pass is the custom shader using "blendFunc" <add>.
| :: "paints" each side of the viewmodel using its modelspace vertex-positions as uv-coordinates, allowing for 3 different "shaders" (with edge blending depending on the normal)
|
|--> [SIDE_PROJECTION]
| :: 2 pass shader. First pass shades the weapon using stock shaders (needs atleast a colorMap), second pass is the custom shader using "blendFunc" <add>.
| :: use only one side of the weapon to "paint" the whole viewmodel (modelspace vertex-positions as uv-coordinates)
|
|--> [3D_PROJECTION]
| :: 2 pass shader. First pass shades the weapon using stock shaders (needs atleast a colorMap), second pass is the custom shader using "blendFunc" <add>.
| :: project "3D" shaders onto a weapon (shaders using cameras / raymarching) (modelspace vertex-positions as uv-coordinates)


[XMODEL]
|--> [FULL_PROJECTION]
| :: 2 pass shader. First pass shades the model using stock shaders (needs atleast a colorMap), second pass is the custom shader using "blendFunc" <add>.
| :: "paints" each side of the model using its modelspace vertex-positions as uv-coordinates, allowing for 3 different "shaders" (with edge blending depending on the normal)


[WORLD]
|--> [_NAME]
| :: 
|
|--> [_NAME]
| :: 


[SKY]
|--> [SINGLE_IMAGE]
| :: the engine requires a cubemap for skies to function
| :: if your shader doesn't make use of any input image, create a 1x1 pixel cubemap with the default sky naming-scheme (_ft, _bk ...)
| :: your cubemap has to somewhat contribute to the final color output of your pixelshader, so do something like this:

	float3 nullsky = texCUBE(skyMapSampler, pixel.worldpos).xyz;
	nullsky *= 0.0001;

	// the sampled sky texture has to contribute to the final color output in some way
	fragment.color = float4(nullsky + fragmentColor, 1.0f);
	return fragment;
	
|--> [SAMPLE_CAMERA_INPUT]
| :: all of the above ^
| :: 3D skybox shader that "moves" with the player to create the illusion of it being real "geometry"

// *
// *
// Creating new templates:

 -- [Material Templates] -------------------------------------------------------------------------------------------------
 1. Create a material template with the following the naming scheme: 
	: ["shadergen_" "whatever_you_like"] in <ShaderGen\deffiles\materials>
 2. ShaderGen will do nothing with material templates besides copying them and print its usage to the console 
 	: (if the material template was specified inside the used techset, see below) 

 -- [Techsets] -------------------------------------------------------------------------------------------------
 1. Create a techset with the following the naming scheme: 
	: ["shadergen_" "type_" "option"] in <ShaderGen\techsets> 
    : Supported types are: 2d, viewmodel, xmodel, world, sky
    : Options are self defined
 2. The first line of your techset should define the material template found in <ShaderGen\deffiles\materials> 
  	: Define your template like so: "// # [template_name.template]" (without "")
	: ShaderGen will not copy/print the material template if you do not specify it within the techset 
 3. ShaderGen will replace "SHADER_NAME" with the specified name when generating shaders. 

 -- [Techniques] -------------------------------------------------------------------------------------------------
 1. Create a technique with the following the naming scheme: 
 	: ["shadergen_" "type_" "option"] in <ShaderGen\techniques> 
	: Make sure your naming scheme matches that of your techset
 2. ShaderGen will replace "SHADER_NAME" with the specified name when generating shaders. 

 -- [Shaders] -------------------------------------------------------------------------------------------------
 1. Create a vertexshader with the following the naming scheme: 
 	: ["vs_3_0_" "shadergen_" "type_" "option"] in <ShaderGen\shaders> 
	: Make sure your naming scheme matches that of your techset/technique
 2. Create a pixelshader with the following the naming scheme:  
 	: ["ps_3_0_" "shadergen_" "type_" "option"] in <ShaderGen\shaders> 
	: Make sure your naming scheme matches that of your techset/technique
 3. ShaderGen will replace "SHADER_NAME" with the specified name when generating shaders. 


// *
// *
// Manually injecting a shader into a model:

-- [XModel] -------------------------------------------------------------------------------------------------
 1. Go into "raw/xmodel"
 2. Search for your gun "viewmodel_gunname" and create a backup
 3. Open "viewmodel_gunname" with notepad++ or a hexeditor and search for a string like "mtl_weapon_desert_eagle_silver" (deagle as example)
 4. Modify all instances of the material string. e.g. "mtl_weapon_desert_eagle_silvex" (only change 1 char (keep the stock length))
 5. Save it

-- [Material] -------------------------------------------------------------------------------------------------
 1. Go into "raw/materials"
 2. Search for "mtl_weapon_desert_eagle_silver" and create a copy
 3. Name it "mtl_weapon_desert_eagle_silvex" (has to match the modified string inside your xmodel)
 4. Open the material with notepad++ or a hexeditor and search for "l_sm_r0c0s0" (should be the first readable string and could be different for some materials, see below)
 5. Replace "l_sm_r0c0s0" with the name you entered for your shader (11 chars long in this case, because "l_sm_r0c0s0" is 11 chars long)