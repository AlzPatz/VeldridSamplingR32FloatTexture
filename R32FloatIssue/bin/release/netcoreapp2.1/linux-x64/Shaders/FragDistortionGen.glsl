#version 330 core

uniform Factors
{
    float MaxHeight;
};

in vec2 FTex;

out float fragHeight;

void main(void)
{
    if(FTex.x *FTex.y >= 0.0)
    {
        fragHeight = 1.0;
    }
    else
    {
        fragHeight = -1.0;
    }
}