#version 330 core

// Input vertex data, different for all executions of this shader.
in vec3 position;
in vec2 texCoord0;
in vec4 color;

uniform mat4 view;
uniform mat4 projection;
uniform mat4 model;

// Output data ; will be interpolated for each fragment.
out vec4 COLOR;
out vec2 UV;
out vec3 WP;

void main(){
	WP = position;
    gl_Position = projection * view *  model * vec4(position,1);
    //gl_Position = viewProjection * model * vec4(position,1);
	COLOR = color;
	UV = texCoord0;
}