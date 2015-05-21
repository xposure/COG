#version 330 core

// Input vertex data, different for all executions of this shader.
in vec3 position;
in vec2 texCoord0;

uniform mat4 viewProjection;
uniform mat4 model;

// Output data ; will be interpolated for each fragment.
out vec2 UV;

void main(){
    gl_Position = viewProjection * model * vec4(position,1);
	UV = texCoord0;
}