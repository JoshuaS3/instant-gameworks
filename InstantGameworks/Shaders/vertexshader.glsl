﻿#version 450 core
#line 2 "vertexshader.glsl"


out vec3 fragPos;
out vec3 fragNorm;
out vec4 objectDiffuse;
layout (location = 0) in vec4 position;
layout (location = 1) in vec3 normal;
layout (location = 2) uniform vec4 diffuse;
layout (location = 3) uniform mat4 modelView;
layout (location = 4) uniform mat4 projection;

void main(void)
{
	fragPos = vec3(modelView * position);
	fragNorm = normalize(normal);
	objectDiffuse = diffuse;

	gl_Position = projection * modelView * position;
}