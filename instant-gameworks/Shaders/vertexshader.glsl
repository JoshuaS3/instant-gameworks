#version 450 core
#line 2 "vertexshader.glsl"


out vec4 fragPos;
out vec3 fragNorm;
out vec3 camera;
layout (location = 0) in vec4 position;
layout (location = 1) in vec3 normal;
layout (location = 2) uniform mat4 modelView;
layout (location = 3) uniform mat4 projection;

void main(void)
{
	fragPos = position;
	fragNorm = normalize(normal);
	camera = normalize(-vec3(projection * modelView * position));

	gl_Position = projection * modelView * position;
}