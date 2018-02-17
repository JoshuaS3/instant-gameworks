#version 450 core
#line 2 "vertexshader.glsl"


struct IGWdirectionalLight
{
	vec3 color;
	vec3 direction;
	float intensity;
};

out vec4 vs_color;
layout (location = 0) in vec4 position;
layout (location = 1) uniform vec4 color;
layout (location = 2) uniform mat4 modelView;
layout (location = 3) uniform mat4 projection;
layout (location = 4) uniform vec4 test;

void main(void)
{
	vs_color = color;
	gl_Position = projection * modelView * position;
}