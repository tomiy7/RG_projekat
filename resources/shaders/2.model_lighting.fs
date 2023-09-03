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
    sampler2D texture_normal1;

    float shininess;
};
in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos;
in vec3 TangentViewPos;
in vec3 TangentFragPos;
in vec3 directDir;

uniform DirectLight directLight;
uniform Material material;

uniform vec3 viewPosition;
uniform bool blinn;

vec3 CalcDirectLight(vec3 directDir, DirectLight light, vec3 normal, vec3 viewDir);

void main()
{
    if(vec4(texture(material.texture_diffuse1, TexCoords)).a < 0.2)
            discard;

    vec3 normal = texture(material.texture_normal1, TexCoords).rgb;
    normal = normalize(normal * 2.0 - 1.0);
    vec3 viewDir = normalize(TangentViewPos - TangentFragPos);
    vec3 result = CalcDirectLight(directDir, directLight, normal, viewDir);

    FragColor = vec4(result, 1.0);
}

vec3 CalcDirectLight(vec3 directDir, DirectLight light, vec3 normal, vec3 viewDir){
    vec3 lightDir = normalize(-directDir);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    if(blinn){
        vec3 halfwayDir = normalize(lightDir + viewDir);
        spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);
    }
    else{
        spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    }

    vec3 ambient = light.ambient * vec3(texture(material.texture_diffuse1, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.texture_diffuse1, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.texture_specular1, TexCoords));
    return (ambient + diffuse + specular);
}