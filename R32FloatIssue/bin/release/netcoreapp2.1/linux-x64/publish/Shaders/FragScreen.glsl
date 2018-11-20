#version 330 core

uniform sampler2D SamplerHeightMap;

in vec2 FTex;

out vec4 fragColor;

void main(void)
{
    float height = texture(SamplerHeightMap, FTex).r;

    float b = 0.0;
    float g = 0.0;

    if(height > 1.0)
    {
        b = height;
    }
    else
    {
        g = -height;
    }
    
    fragColor = vec4(0.0, g, b, 1.0);
}