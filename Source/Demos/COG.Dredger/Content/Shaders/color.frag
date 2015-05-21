#version 330 core

// Interpolated values from the vertex shaders
in vec4 COLOR;

out vec4 color;
 

void main(){
    // Output color = color specified in the texture at the specified uv
    color =  COLOR;
}