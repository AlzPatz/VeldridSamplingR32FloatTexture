#version 330 core

in vec2 TexCoordFragment;

out float fragValue; //Out as a float (assumed correct for R32 Signed Float)

void main(void)
{
    if(TexCoordFragment.x < 0.5)
    {
        fragValue = 1.0;
    }
    else
    {
        fragValue = -1.0;
    }
}