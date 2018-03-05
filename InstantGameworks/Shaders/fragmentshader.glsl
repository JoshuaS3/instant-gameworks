#version 450 core
#line 2 "fragmentshader.glsl"

struct directionalLight
{
	vec4 diffuseColor;
	vec4 specularColor;
	vec4 ambientColor;
	vec4 emitColor;
	float intensity;
	vec3 direction;
	int lightActive;
};
struct pointLight
{
	vec4 diffuseColor;
	vec4 specularColor;
	vec4 ambientColor;
	vec4 emitColor;
	float intensity;
	float radius;
	vec3 position;
	int lightActive;
};

in vec4 fragPos;
in vec3 fragNorm;
in vec3 eye;
layout (location = 4) uniform mat4 rotation;
layout (location = 5) uniform vec4 diffuse;
layout (location = 6) uniform vec4 specular;
layout (location = 7) uniform vec4 ambient;
layout (location = 8) uniform vec4 emit;
layout (location = 100) uniform directionalLight dLights[8];
layout (location = 156) uniform pointLight pLights[108];
out vec4 color;

void main(void)
{
	vec3 adjustedNormal = normalize(vec3(rotation * vec4(fragNorm, 1)));
	vec4 final_color = ambient;

	for (int i = 0; i < 8; i++) {
		if (dLights[i].lightActive == 1) {
			vec4 diffuseColor = (diffuse + dLights[i].diffuseColor) * max(  dot(  -dLights[i].direction, adjustedNormal  ),   0.0 );
	
			vec3 e = normalize(eye);
			vec3 r = normalize(-reflect(-dLights[i].direction, adjustedNormal));
			vec4 specColor = (specular + dLights[i].specularColor) * pow(max(min(dot(e, r), 1.0), 0.0), 0.3*dLights[i].intensity);
			specColor = clamp(specColor, 0.0, 1.0);

			final_color += diffuseColor + specColor + emit;
		}
	}

	for (int i = 0; i < 108; i++)
	{
		if (pLights[i].lightActive == 1) {
			
		}
	}

	color = final_color;
}