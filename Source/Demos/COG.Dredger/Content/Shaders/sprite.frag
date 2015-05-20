#version 330 core

// Interpolated values from the vertex shaders
in vec2 UV;
in vec4 c;

out vec4 color;
 
// Values that stay constant for the whole mesh.
uniform sampler2D myTextureSampler;

void main(){
    // Output color = color specified in the texture at the specified uv
    color.rgb = texture(myTextureSampler, UV).rgb * c.rgb;
	color.a = c.a;
	//color = color * c;
	//color.rgba = c;

}