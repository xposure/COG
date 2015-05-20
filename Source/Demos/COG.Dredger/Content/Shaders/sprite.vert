#version 330 core

// Input vertex data, different for all executions of this shader.
layout(location = 0) in vec3 vertexPosition_modelspace;
layout(location = 1) in vec2 vertexUV;
layout(location = 2) in vec4 vertexColor;

uniform mat4 MVP;

// Output data ; will be interpolated for each fragment.
out vec2 UV;
out vec4 c;

void main(){
 
    // Output position of the vertex, in clip space : MVP * position
    vec4 v = vec4(vertexPosition_modelspace,1); // Transform an homogeneous 4D vector, remember ?
    gl_Position = MVP * v;
	UV = vertexUV;
	c = vertexColor
}