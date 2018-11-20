#version 330 core

in vec2 Position;
in vec2 TexCoordVertex;

out vec2 TexCoordFragment;

void main(void)
{
    TexCoordFragment = TexCoordVertex;
    gl_Position = vec4(Position.x, Position.y, 0.5, 1.0);
}