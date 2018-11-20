using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using Veldrid.StartupUtilities;
using Veldrid.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace EffectDemo
{
    public class Demo
    {
        private Sdl2Window _window;
        private GraphicsDevice _device;
        private DisposeCollectorResourceFactory _factory;

        private DeviceBuffer _vbPosition;
        private ShaderSetDescription _shaderSet;
        
        private Pipeline _pipeLine;

        private CommandList _cl;        

        private ResourceSet _R32FloatTextureResourceSet; //Resource set for R32Float format texture created by the R32FloatTextureGenerator

        public void Run()
        {
            Init();

            Loop();

            ReleaseResources();
        }

        private void Init()
        {
            //Sets up basic full screen quad render of a single texture

            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "Potential Issue Sampling R32 Float Texture"
            };

            _window = VeldridStartup.CreateWindow(ref windowCI);

            _device = VeldridStartup.CreateGraphicsDevice(_window); 

            _factory = new DisposeCollectorResourceFactory(_device.ResourceFactory);

            _cl = _factory.CreateCommandList();

            Vertex2DTextured[] fullScreenQuadVertices = 
            {
                    new Vertex2DTextured { Position = new Vector2(-1.0f, 1.0f), TexCoord = new Vector2(0.0f, 1.0f) }, 
                    new Vertex2DTextured { Position = new Vector2(1.0f, 1.0f), TexCoord = new Vector2(1.0f, 1.0f) }, 
                    new Vertex2DTextured { Position = new Vector2(1.0f, -1.0f), TexCoord = new Vector2(1.0f, 0.0f) }, 

                    new Vertex2DTextured { Position = new Vector2(-1.0f, 1.0f), TexCoord = new Vector2(0.0f, 1.0f) }, 
                    new Vertex2DTextured { Position = new Vector2(1.0f, -1.0f), TexCoord = new Vector2(1.0f, 0.0f) }, 
                    new Vertex2DTextured { Position = new Vector2(-1.0f, -1.0f), TexCoord = new Vector2(0.0f, 0.0f) }
            };

            _vbPosition = _factory.CreateBuffer(new BufferDescription(6 * Vertex2DTextured.SizeInBytes, BufferUsage.VertexBuffer));
            _device.UpdateBuffer(_vbPosition, 0, fullScreenQuadVertices);

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
            var fragmentShader = Utility.LoadShader("FragOnScreen", ShaderStages.Fragment, _device);

            _shaderSet = new ShaderSetDescription(
                new[]
                {
                    vertexLayout
                },
                new[]
                {
                    vertexShader, fragmentShader
                }
            );

            var textureLayout = CreateTextureResourceLayout();

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
                ResourceLayouts = new ResourceLayout[] { textureLayout },
                ShaderSet = _shaderSet,
                Outputs = _device.SwapchainFramebuffer.OutputDescription
            };

            _pipeLine = _factory.CreateGraphicsPipeline(pipelineDescription);

            //This creates a one time use render pass that draws an R32 Float format texture (split vertically, half -1 and half +1). 
            //Stores (returns) reference to texture resource set to use in shader later
            var r32FloatTextureGenerator = new R32FloatTextureGenerator(_device, _factory);
            _R32FloatTextureResourceSet = r32FloatTextureGenerator.Generate();
        }

        private ResourceLayout CreateTextureResourceLayout()
        {
            return _factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SamplerR32FloatTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("texture", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            );
        }

        private void Loop()
        {
            while(_window.Exists)
            {
                _window.PumpEvents();
                Render();
            }
            _factory.DisposeCollector.DisposeAll();
        }

        private void Render()
        {
            _cl.Begin();

            _cl.SetPipeline(_pipeLine);

            _cl.SetFramebuffer(_device.SwapchainFramebuffer);

            _cl.SetVertexBuffer(0, _vbPosition);

            //Bind the R32 float format texture
            _cl.SetGraphicsResourceSet(0, _R32FloatTextureResourceSet);

            _cl.ClearColorTarget(0, RgbaFloat.CornflowerBlue);

            _cl.Draw(6, 1, 0, 0);

            _cl.End();

            _device.SubmitCommands(_cl);

            _device.SwapBuffers();

            _device.WaitForIdle();
        }

        private void ReleaseResources()
        {
            _factory.DisposeCollector.DisposeAll();
            _device.Dispose();
        }
    }
}