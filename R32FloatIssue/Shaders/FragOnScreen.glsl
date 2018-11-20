#version 330 core

uniform sampler2D SamplerR32FloatTexture;

in vec2 TexCoordFragment;

out vec4 fragColor;

void main(void)
{
    float sampledValue = texture(SamplerR32FloatTexture, TexCoordFragment).r;

    float b = 0.0;
    float g = 0.0;

    if(sampledValue > 0)
    {
        b = sampledValue;
    }
    else
    {
        g = -sampledValue;
    }

    fragColor = vec4(0.0, g, b, 1.0);

    //Comment above coode and use below to generate the result I am expecting
    /*
    if(TexCoordFragment.x < 0.5)
    {
        fragColor = vec4(0.0, 0.0, 1.0, 1.0);
    }
    else
    {
        fragColor = vec4(0.0, 1.0, 0.0, 1.0);
    }
    */
}