#version 330 core

// Input vertex data, different for all executions of this shader.
in vec3 position;
in vec4 color;

uniform mat4 viewProjection;
uniform mat4 model;

// Output data ; will be interpolated for each fragment.
out vec4 COLOR;

void main(){
    gl_Position = viewProjection * model * vec4(position,1);
	COLOR = color;
}