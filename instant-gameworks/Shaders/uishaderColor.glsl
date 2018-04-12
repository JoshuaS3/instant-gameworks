#version 450 core
#line 2 "uishaderColor.glsl"

layout (location = 1) uniform vec4 solidColor;
out vec4 color;


void main(void)
{
	color = solidColor;
}
