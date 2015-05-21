#version 330 core

// Interpolated values from the vertex shaders
in vec2 UV;
in vec4 COLOR;

out vec4 color;
 
// Values that stay constant for the whole mesh.
uniform sampler2D myTextureSampler;

void main(){
    // Output color = color specified in the texture at the specified uv
    color.rgba = texture(myTextureSampler, UV).rgba * COLOR.rgba;
	if(color.a == 0)
		discard;
}