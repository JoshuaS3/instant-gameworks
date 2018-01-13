#version 440 core
#line 2 "fragmentshader.glsl"

in vec4 frag_color;
out vec4 color;

void main(void)
{
	color = frag_color;
}