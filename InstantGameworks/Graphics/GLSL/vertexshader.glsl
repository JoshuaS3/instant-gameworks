#version 450 core
#line 2 "vertexshader.glsl"

layout (location = 0) in vec4 position;
layout (location = 1) in vec4 color;
out vec4 vs_color;
layout (location = 20) uniform mat4 modelView;

void main(void)
{
	vs_color = color;
	gl_Position = modelView * position;
}