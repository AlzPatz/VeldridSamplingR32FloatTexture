using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.Utilities;

namespace EffectDemo
{
    public class R32FloatTextureGenerator
    {
        private GraphicsDevice _device;
        private DisposeCollectorResourceFactory _factory;

        public R32FloatTextureGenerator(GraphicsDevice device, DisposeCollectorResourceFactory factory)
        {
            _device = device;
            _factory = factory;    
        }

        public ResourceSet Generate()
        {
            Vertex2DTextured[] fullscreenQuadVertices =
            {
                    new Vertex2DTextured { Position = new Vector2(-1.0f, 1.0f), TexCoord = new Vector2(0.0f, 1.0f) }, 
                    new Vertex2DTextured { Position = new Vector2(1.0f, 1.0f), TexCoord = new Vector2(1.0f, 1.0f) }, 
                    new Vertex2DTextured { Position = new Vector2(1.0f, -1.0f), TexCoord = new Vector2(1.0f, 0.0f) }, 

                    new Vertex2DTextured { Position = new Vector2(-1.0f, 1.0f), TexCoord = new Vector2(0.0f, 1.0f) }, 
                    new Vertex2DTextured { Position = new Vector2(1.0f, -1.0f), TexCoord = new Vector2(1.0f, 0.0f) }, 
                    new Vertex2DTextured { Position = new Vector2(-1.0f, -1.0f), TexCoord = new Vector2(0.0f, 0.0f) }
            };

            var vBuffer = _factory.CreateBuffer(new BufferDescription(12 * Vertex2DTextured.SizeInBytes, BufferUsage.VertexBuffer));
            _device.UpdateBuffer(vBuffer, 0, fullscreenQuadVertices);

            var vertexLayout  = new VertexLayoutDescription
            (
                16,
                0,
                new VertexElementDescription[] {
                    new VertexElementDescription("Position", VertexElementFormat.Float2, VertexElementSemantic.Position),
                    new VertexElementDescription("TexCoordVertex", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate)
                }
            );

            var vertexShader = Utility.LoadShader("Vertex", ShaderStages.Vertex, _device);
            var fragmentShaderDistortionGen = Utility.LoadShader("FragR32TextureGen", ShaderStages.Fragment, _device);

            var shaderSet = new ShaderSetDescription(
                new[]
                {
                    vertexLayout
                },
                new[]
                {
                    vertexShader, fragmentShaderDistortionGen
                }
            );

            var pipelineDescription = new GraphicsPipelineDescription()
            {
                BlendState = BlendStateDescription.SingleAlphaBlend,
                DepthStencilState = new DepthStencilStateDescription(
                    depthTestEnabled: false, 
                    depthWriteEnabled: false,
                    comparisonKind: ComparisonKind.LessEqual),
                RasterizerState = new RasterizerStateDescription(
                    cullMode: FaceCullMode.None, 
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true, 
                    scissorTestEnabled: false
                ),
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceLayouts = new ResourceLayout[] { },
                ShaderSet = shaderSet,
                Outputs = new OutputDescription(null, new OutputAttachmentDescription(PixelFormat.R32_Float))
            };

            var pipeLine = _factory.CreateGraphicsPipeline(pipelineDescription);

            var dimension = 128u;

            var texture = _factory.CreateTexture(TextureDescription.Texture2D(
                                    dimension,
                                    dimension,
                                    1,
                                    1,
                                    PixelFormat.R32_Float, TextureUsage.RenderTarget | TextureUsage.Sampled
            ));

            var view = _factory.CreateTextureView(texture);

            var framebuffer = _factory.CreateFramebuffer(new FramebufferDescription(null, texture));

            var sampler = _factory.CreateSampler(new SamplerDescription(SamplerAddressMode.Clamp, SamplerAddressMode.Clamp, SamplerAddressMode.Clamp, SamplerFilter.MinPoint_MagPoint_MipPoint, null, 4, 0, 0, 0, SamplerBorderColor.TransparentBlack));

            var textureResourceLayout = _factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("texSampler", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("tex", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            );

            var r32FloatTextureResourceSet = _factory.CreateResourceSet(new ResourceSetDescription(
                textureResourceLayout,
                view,
                sampler
            ));

            var cl = _factory.CreateCommandList();

            cl.Begin();

            cl.SetFramebuffer(framebuffer);

            cl.SetVertexBuffer(0, vBuffer);

            cl.SetPipeline(pipeLine);

            cl.Draw(6, 1, 0, 0);

            cl.End();

            _device.SubmitCommands(cl);
            
            _device.SwapBuffers();

            _device.WaitForIdle();

            return r32FloatTextureResourceSet;
        }
    }
}