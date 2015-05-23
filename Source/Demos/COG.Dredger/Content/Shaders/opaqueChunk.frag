#version 330 core

// Interpolated values from the vertex shaders
in vec4 COLOR;
in vec2 UV;
in vec3 WP;

out vec4 color;
 
float edgeDarken(float p){
	const float edgeThickness = 0.1f;
	const float inverseEdgeThickness = 1f - edgeThickness;
	
	float xf = fract(p);

	if(xf < edgeThickness)
		return edgeThickness - xf;
	else if(xf > inverseEdgeThickness)
		return (xf - inverseEdgeThickness);

	return 0f;
}

float aoDarken(float x){
    const vec4 aoCurve = vec4(0.0f, 0.5f, 0.8f, 1.0f);
	
	if(x < 0.33)
		x = mix(aoCurve.x, aoCurve.y, max(0.0, x) / 0.33);
	else if(x < 0.66)
		x = mix(aoCurve.y, aoCurve.z, (x - 0.33) / 0.33);
	else
		x = mix(aoCurve.z, aoCurve.w, (min(1.0, x) - 0.66) / 0.33);
				
	x = x / 2 + 0.5;
	return x;
}

void main(){
	float aoAmount = aoDarken(UV.x);
	color.rgb = COLOR.rgb * aoAmount;
	

	float edgeAmount = edgeDarken(WP.x) +
						edgeDarken(WP.y) +
						edgeDarken(WP.z);

	color.rgb *= vec3(1f - (edgeAmount + edgeAmount) / 1.5f);

	color.a = COLOR.a;
}