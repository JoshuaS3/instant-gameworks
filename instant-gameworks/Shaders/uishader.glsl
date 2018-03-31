#version 450 core
#line 2 "uishader.glsl"

out vec4 vsColor;
layout (location = 0) uniform vec4 solidColor;
layout (location = 1) in vec4 position;


void main(void)
{
	vsColor = solidColor;
	gl_Position = position;
}
