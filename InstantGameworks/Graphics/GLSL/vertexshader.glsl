#version 440 core
#line 2 "vertexshader.glsl"

layout (location = 0) in float time;
out vec4 frag_color;

void main(void)
{
	gl_Position = vec4(sin(time) * 0.5, cos(time) * 0.5, 0.0, 1.0);
	frag_color = vec4(sin(time) * 0.25 + 0.75, cos(time) * 0.25 + 0.75, 0.0, 0.0);
}