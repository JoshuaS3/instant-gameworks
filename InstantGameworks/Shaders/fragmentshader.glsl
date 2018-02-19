#version 450 core
#line 2 "fragmentshader.glsl"

in vec3 fragPos;
in vec3 fragNorm;
in vec4 objectDiffuse;
out vec4 color;

void main(void)
{
	vec3 dir = normalize(vec3(0, 10, 5) - fragPos);
	vec4 vs_color = objectDiffuse * max(dot(dir, fragNorm), 0.0) + vec4(0, 0, 0.1, 0.0);
	color = vs_color;
}