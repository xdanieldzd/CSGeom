#version 330

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

uniform sampler2D material_texture;
uniform vec4 material_ambient;
uniform vec4 material_diffuse;
uniform vec4 material_specular;

uniform vec3 sun_position;
uniform vec3 sun_lightColor;
uniform float ambient_intensity;

uniform int discard_opaque;
uniform int enable_light;

in vec3 frag_position;
in vec3 frag_normal;
in vec4 frag_color;
in vec2 frag_texCoord;

out vec4 out_finalColor;

void main(void)
{
    vec4 surface_color = frag_color * texture2D(material_texture, frag_texCoord);

    if (discard_opaque == 1 && surface_color.a == 1.0) discard;
    else if (discard_opaque == 0 && surface_color.a < 1.0) discard;
    else
    {
        if (enable_light == 1)
        {
            vec3 normal = normalize(transpose(inverse(mat3(modelview_matrix))) * frag_normal);
            float diffuse_intensity = max(0.0, dot(normalize(normal), normalize(sun_position)));

            out_finalColor = surface_color * vec4(sun_lightColor * (ambient_intensity + diffuse_intensity), 1.0);
            //out_finalColor = vec4(normal, 1.0);
        }
        else
        {
            out_finalColor = surface_color;
        }
    }
}
