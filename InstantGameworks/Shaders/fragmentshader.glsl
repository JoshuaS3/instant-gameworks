#version 450 core
#line 2 "fragmentshader.glsl"

in vec4 vs_color;
out vec4 color;

void main(void)
{
	color = vs_color;
}