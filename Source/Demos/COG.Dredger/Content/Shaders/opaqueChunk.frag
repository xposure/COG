#version 330 core

// Interpolated values from the vertex shaders
in vec4 COLOR;
in vec2 UV;

out vec4 color;
 

void main(){
    // Output color = color specified in the texture at the specified uv
    const vec4 aoCurve = vec4(0.0f, 0.5f, 0.8f, 1.0f);
	float x = UV.x;
	
	if(x < 0.33)
		x = mix(aoCurve.x, aoCurve.y, max(0.0, x) / 0.33);
	else if(x < 0.66)
		x = mix(aoCurve.y, aoCurve.z, (x - 0.33) / 0.33);
	else
		x = mix(aoCurve.z, aoCurve.w, (min(1.0, x) - 0.66) / 0.33);
				
	x = x / 2 + 0.5;

	color.a = 1f;
	//color.a = 0.5f;
	color.rgb = COLOR.rgb * x;
}