#version 330 core
out vec4 FragColor;

struct DirectLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_specular1;

    float shininess;
};
in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos;

uniform DirectLight directLight;
uniform Material material;

uniform vec3 viewPosition;

vec3 CalcDirectLight(DirectLight light, vec3 normal, vec3 viewDir);

void main()
{
    vec3 normal = normalize(Normal);
    vec3 viewDir = normalize(viewPosition - FragPos);
    vec3 result = CalcDirectLight(directLight, normal, viewDir);
    if(vec4(texture(material.texture_diffuse1, TexCoords)).a < 0.4)
        discard;

    FragColor = vec4(result, 1.0);
}

vec3 CalcDirectLight(DirectLight light, vec3 normal, vec3 viewDir){
    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(normal, lightDir), 0.0);

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    vec3 ambient = light.ambient * vec3(texture(material.texture_diffuse1, TexCoords));
    vec3 diffuse = light.diffuse * vec3(texture(material.texture_diffuse1, TexCoords));
    vec3 specular = light.specular * vec3(texture(material.texture_specular1, TexCoords));
    return (ambient + diffuse + specular);
}