#version 330 core

in vec3 Position;
in vec2 VTex;

out vec2 FTex;
out float Intensity;

void main(void)
{
    FTex = VTex;
    Intensity = Position.z;
    gl_Position = vec4(Position.x, Position.y, 0.5, 1.0);
}