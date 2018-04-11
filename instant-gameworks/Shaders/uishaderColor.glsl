#version 450 core
#line 2 "uishaderColor.glsl"

in vec4 vsColor;
out vec4 color;


void main(void)
{
	color = vsColor;
}
