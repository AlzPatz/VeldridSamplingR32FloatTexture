using System;
using System.IO;
using Veldrid;

namespace EffectDemo
{
    public static class Utility
    {
        public static Shader LoadShader(string name, ShaderStages stage, GraphicsDevice device)
        {
            string extension = null;
            switch (device.BackendType)
            {
                case GraphicsBackend.Direct3D11:
                    extension = "hlsl.bytes";
                    break;
                case GraphicsBackend.Vulkan:
                    extension = "spv";
                    break;
                case GraphicsBackend.OpenGL:
                    extension = "glsl";
                    break;
                default: throw new System.InvalidOperationException();
            }

            string entryPoint = stage == ShaderStages.Vertex ? "VS" : "FS";
            string path = Path.Combine(System.AppContext.BaseDirectory, "Shaders", $"{name}.{extension}");
            byte[] shaderBytes = File.ReadAllBytes(path);

            return device.ResourceFactory.CreateShader(new ShaderDescription(stage, shaderBytes, entryPoint));
        }
    }
}