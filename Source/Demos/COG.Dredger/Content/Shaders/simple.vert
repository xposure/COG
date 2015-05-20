#version 330 core

// Input vertex data, different for all executions of this shader.
in vec3 position;
in vec2 texCoord0;

uniform mat4 MVP;

// Output data ; will be interpolated for each fragment.
out vec2 UV;

void main(){
 
    // Output position of the vertex, in clip space : MVP * position
    vec4 v = vec4(position,1); // Transform an homogeneous 4D vector, remember ?
    gl_Position = MVP * v;
	UV = texCoord0;
}