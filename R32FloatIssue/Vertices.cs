using System.Numerics;
using Veldrid;

namespace EffectDemo
{
    public struct Vertex2DTextured
    {
        public Vector2 Position;
        public Vector2 TexCoord;
        public const uint SizeInBytes = 16;
    }
}