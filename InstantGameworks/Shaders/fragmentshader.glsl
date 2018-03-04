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
};

in vec4 fragPos;
in vec3 fragNorm;
in vec3 eye;
layout (location = 2) uniform vec4 diffuse;
layout (location = 5) uniform mat4 rotation;
layout (location = 100) uniform directionalLight dLights[8];
layout (location = 148) uniform pointLight pLights[256];
out vec4 color;

void main(void)
{
	vec3 adjustedNormal = normalize(vec3(rotation * vec4(fragNorm, 1)));
	vec3 lightDir = -vec3(0,-2,-1.5);
	vec4 vs_color = diffuse * max(  dot(  lightDir, adjustedNormal  ),   0.0 ) + vec4(0.2, 0.2, 0.2, 0.0);
	
	vec3 e = normalize(-eye);
	vec3 r = normalize(-reflect(lightDir, adjustedNormal));
	vec4 spec = vec4(0.8, 0.8, 0.8, 0.0) * pow(max(dot(r, e), 0.0), 0.3*64);
	spec = clamp(spec, 0.0, 1.0);

	color = vs_color + spec;
}