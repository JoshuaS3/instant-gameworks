#version 450 core
#line 2 "vertexshader.glsl"


struct directionalLight
{
	vec4 color;
	vec3 direction;
	float intensity;
};

out vec4 vs_color;
layout (location = 0) in vec4 position;
layout (location = 1) in vec3 normal;
layout (location = 2) uniform vec4 color;
layout (location = 3) uniform mat4 modelView;
layout (location = 4) uniform mat4 projection;
layout (location = 5) uniform directionalLight dLights[8];

void main(void)
{
	normalize(normal);
	vec3 test = -vec3(0.2, -0.5, 0.5);
	float d = max(dot(normal, test), 0);
	vs_color = (color * d) + (vec4(0.2, 0.2, 0.2, 1));
	gl_Position = projection * modelView * position;
}