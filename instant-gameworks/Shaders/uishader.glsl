#version 450 core
#line 2 "uishader.glsl"

out vec4 vsColor;
layout (location = 0) in vec4 position;
layout (location = 1) uniform vec4 solidColor;


void main(void)
{
	vsColor = solidColor;
	gl_Position = position;
}
