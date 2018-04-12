#version 450 core
#line 2 "uishader.glsl"

layout (location = 0) in vec4 position;


void main(void)
{
	gl_Position = position;
}
