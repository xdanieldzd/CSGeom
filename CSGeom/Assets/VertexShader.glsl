#version 330

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

layout(location = 0) in vec3 in_position;
layout(location = 1) in vec3 in_normal;
layout(location = 2) in vec4 in_color;
layout(location = 3) in vec2 in_texCoord;
layout(location = 4) in vec4 in_boneIDs;
layout(location = 5) in vec4 in_boneWeights;

out vec3 frag_position;
out vec3 frag_normal;
out vec4 frag_color;
out vec2 frag_texCoord;

void main(void)
{
    frag_position = in_position;
    frag_normal = in_normal;
    frag_color = in_color;
    frag_texCoord = in_texCoord;

    gl_Position = projection_matrix * modelview_matrix * vec4(in_position, 1.0);

    //frag_color = vec4(in_boneIDs.x /128.0, in_boneIDs.y/128.0,0,1);
	//frag_color = vec4(in_boneWeights.x, in_boneWeights.y, in_boneWeights.z, 1.0);
}
