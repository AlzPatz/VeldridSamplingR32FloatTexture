#version 330 core

uniform sampler2D texSampler;

in vec2 FTex;
in float Intensity;

out vec4 fragColor;

void main(void)
{
    fragColor = Intensity * texture(texSampler, FTex);    
}