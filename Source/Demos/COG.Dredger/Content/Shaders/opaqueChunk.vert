#version 330 core

// Input vertex data, different for all executions of this shader.
in vec3 position;
in vec2 texCoord0;
in vec4 color;

uniform mat4 MVP;

// Output data ; will be interpolated for each fragment.
out vec4 COLOR;
out vec2 UV;

void main(){
    vec4 v = vec4(position,1); // Transform an homogeneous 4D vector, remember ?
    gl_Position = MVP * v;

	COLOR = color;
	UV = texCoord0;
}