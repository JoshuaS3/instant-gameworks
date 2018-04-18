/*  Copyright (c) Joshua Stockin 2018
 *
 *  This file is part of Instant Gameworks.
 *
 *  Instant Gameworks is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Instant Gameworks is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Instant Gameworks.  If not, see <http://www.gnu.org/licenses/>.
 */


#version 450 core
#line 2 "vertexshader.glsl"


out vec4 fragPos;
out vec3 fragNorm;
out vec3 camera;
layout (location = 0) in vec4 position;
layout (location = 1) in vec3 normal;
layout (location = 2) uniform mat4 modelView;
layout (location = 3) uniform mat4 projection;
layout (location = 9) uniform mat4 projectionNoRotation;

void main(void)
{
	fragPos = position;
	fragNorm = normalize(normal);
	camera = normalize(vec3(0.0, 0.0, 0.0) - (projectionNoRotation * modelView * position).xyz);
	gl_Position = projection * modelView * position;
}