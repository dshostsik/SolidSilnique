using System;
using GUIRESOURCES;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolidSilnique.Core;
using SolidSilnique.Core.Components;
using SolidSilnique.GameContent;
// Use this to prevent conflicts with System.Numerics.Vector3
using Vector3 = Microsoft.Xna.Framework.Vector3;
// Use this to prevent conflicts with Microsoft.Xna.Framework.Graphics.DirectionalLight
using DirectionalLight = SolidSilnique.Core.DirectionalLight;

namespace SolidSilnique
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;

        private SpriteBatch _spriteBatch;

        private GUI _gui;

        private Input _input;

        //FPS Counter
        private readonly FrameCounter counter;
        private Vector2 frameraterCounterPosition;

        // Model-View-Projection
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;

        // For shadows
        private Matrix _lightView;
        private Matrix _lightProjection;
        private Matrix _lightViewProjection;
        private RenderTarget2D shadowMapRenderTarget;

        private SpriteFont _font;
        private SpriteBatch _text;

        private Skybox _skybox;
        private EnvironmentObject _enviro;


        private bool firstMouse;

        private float lastX;
        private float lastY;

        int scrollWheelValue;
        int currentScrollWheelValue;


        // create bg
        private Rectangle screenBounds;

        // TODO: Remove when unnecessary
        // experimental colors;
        private Vector4 dirlight_ambient;
        private Vector4 dirlight_diffuse;
        private Vector4 dirlight_specular;

        private Vector3 spotlight_position;
        private Vector3 pointlight_position;

        private DirectionalLight testDirectionalLight;
        private Vector3 sunPosition;

        private LightsManagerComponent manager;

        private PointLight testPointLight;
        private GameObject testPointLightGameObject;
        private Spotlight testSpotlight;
        private GameObject testSpotlightGameObject;


        // Custom shader
        //private Effect customEffect;


        private Shader shader;
        private Shader shadowShader;

        public bool useCulling = false;


        private Texture2D _normalMap;


        private Texture2D _defaultRoughnessMap;
        private Texture2D _defaultAOMap;

        private BasicEffect _debugEffect;

        private bool useDebugWireframe = false;

        private bool useNormalMap = true;

        GameTime gameTime;
        private LeafParticle _leafSystem;




        /// <summary>
        /// Constructor
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            Mouse.SetCursor(MouseCursor.Crosshair);
            counter = new FrameCounter();
            scrollWheelValue = 0;
        }

        /// <summary>
        /// Add your initialization logic here
        /// </summary>
        protected override void Initialize()
        {
            //DISPLAY SETUP

            Window.AllowUserResizing = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.IsFullScreen = false;
            Window.IsBorderless = true;
            // _graphics.HardwareModeSwitch = true;
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.SynchronizeWithVerticalRetrace = true; //VSync
            //_graphics.SynchronizeWithVerticalRetrace = false; // disabled VSync for uncapped FPS
            _graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
            _graphics.ApplyChanges();


            // Create camera
            //TODO delete
            //TODO delete
            // matrices initialisations
            _world = Matrix.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.Up); //TODO delete

            // Resize world matrix
            _world = Matrix.CreateScale(1.0f) * _world;

            firstMouse = true;

            screenBounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            // Load shaders
            shader = new Shader("Shaders/CustomShader",
                GraphicsDevice,
                this,
                "BasicColorDrawingWithLights");

            shadowShader = new Shader("Shaders/ShadowMapShader",
                GraphicsDevice,
                this,
                "ShadeTheSceneRightNow");
            shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, 1024, 1024, false, SurfaceFormat.Color,
                DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);


            manager = new LightsManagerComponent(shader);

            dirlight_ambient = new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
            dirlight_diffuse = new Vector4(0.8f, 0.8f, 0f, 1.0f);
            dirlight_specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            spotlight_position = new Vector3(-15.0f, 0.0f, 0.0f);
            pointlight_position = new Vector3(10.0f, 0.0f, 0.0f);

            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                500f);


            testDirectionalLight = new DirectionalLight(new Vector3(1, -1, 0));
            testDirectionalLight.AmbientColor = dirlight_ambient;
            testDirectionalLight.DiffuseColor = dirlight_diffuse;
            testDirectionalLight.SpecularColor = dirlight_specular;

            testPointLight = new PointLight(0.022f, 0.0019f, 1);
            testPointLight.AmbientColor = dirlight_ambient;
            testPointLight.DiffuseColor = dirlight_diffuse;
            testPointLight.SpecularColor = dirlight_specular;

            testPointLightGameObject = new GameObject("Pointlight0");
            testPointLight.gameObject = testPointLightGameObject;
            testPointLightGameObject.AddComponent(testPointLight);


            testSpotlight = new Spotlight(0.007f, 0.0002f, 1, new Vector3(-10, 0, 0), 5.5f, 7.5f);
            testSpotlightGameObject = new GameObject("Spotlight0");
            testSpotlight.gameObject = testSpotlightGameObject;
            testSpotlightGameObject.AddComponent(testSpotlight);


            testDirectionalLight.Enabled = 1;
            testPointLight.Enabled = 1;
            testSpotlight.Enabled = 1;

            sunPosition = new Vector3(50.0f, 50.0f, 0.0f);
            //testSpotlight.Enabled = false;

            manager.AddPointLight(testPointLight);
            manager.AddSpotLight(testSpotlight);
            manager.DirectionalLight = testDirectionalLight;
            manager.DirectionalLightPosition = sunPosition;
            manager.CreateNewPointLight();
            testPointLightGameObject.transform.position = pointlight_position;
            testSpotlightGameObject.transform.position = spotlight_position;

            manager.Start();

            //EngineManager.scene = new TestScene();

            EngineManager.scene = new ProceduralTest();

            _skybox = new Skybox();
            _skybox.Setup(Content, _graphics, GraphicsDevice, _projection);

            _enviro = new EnvironmentObject();

            _input = new Input(this);
            base.Initialize();
        }

        /// <summary>
        /// Use this.Content to load your game content here
        /// </summary>
        protected override void LoadContent()
        {
            // Load the model


            _font = Content.Load<SpriteFont>("Megafont");
            _text = new SpriteBatch(GraphicsDevice);


            _spriteBatch = new SpriteBatch(GraphicsDevice);

            EngineManager.wireframeEffect = new BasicEffect(GraphicsDevice) { VertexColorEnabled = true };
            EngineManager.normalMap = Content.Load<Texture2D>("Textures/normal_map");
            EngineManager.defaultRoughnessMap = Content.Load<Texture2D>("Textures/default_roughness");
            EngineManager.defaultAOMap = Content.Load<Texture2D>("Textures/default_ao");


            EngineManager.scene.LoadContent(Content);
            EngineManager.scene.Setup();

            EngineManager.scene.mainCamera.mouseMovement(0, 0, 0);

            _enviro.Generate("Map1", Content, _graphics.GraphicsDevice);
            

            EngineManager.Start();

            // Initialize GPU leaf particles
            _leafSystem = new LeafParticle(maxParticles: (int)2e+3)
            {
                _game = this
            };
            _leafSystem.LoadContent(GraphicsDevice, Content);
        }

        /// <summary>
        /// Function defining mouse behaviour
        /// </summary>
        /// <param name="gameTime">Object containing time values</param>


        private readonly Vector3 _tpcOffset = new Vector3(0, 5, -10);
        private readonly Vector3 _tpcLookOffset = new Vector3(0, 2, 0);

        /// <summary>
        /// Add your update logic here
        /// </summary>
        /// <param name="gameTime">Object containing time values</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Get current camera view
            //_view = EngineManager.scene.mainCamera.getViewMatrix(); //TODO Delete
            if (EngineManager.scene.mainCamera == EngineManager.scene.TPCamera)
            {
                // Monster’s world position
                Vector3 monsterPos = EngineManager.scene.TPCamera.CameraPosition;
                // Place camera behind & above
                Vector3 camPos = monsterPos + _tpcOffset;
                // Aim slightly down at the monster
                Vector3 lookAt = monsterPos + _tpcLookOffset;
                _view = Matrix.CreateLookAt(camPos, lookAt, Vector3.Up);
            }
            else
            {
                // Free-cam’s usual view
                _view = EngineManager.scene.mainCamera.getViewMatrix();
            }

            _lightView = Matrix.CreateLookAt(sunPosition, sunPosition + testDirectionalLight.Direction, Vector3.Up);
            _lightProjection = Matrix.CreateOrthographic(200, 200, 0.1f, 100f);
            _lightViewProjection = _lightView * _lightProjection;

            // Rotate object
            //_world *= Matrix.CreateRotationY(MathHelper.ToRadians(gameTime.ElapsedGameTime.Milliseconds * 0.01f));

            // Control FOV and perspective settings
            currentScrollWheelValue = Mouse.GetState().ScrollWheelValue;
            if (scrollWheelValue != currentScrollWheelValue)
            {
                //if (scrollWheelValue - currentScrollWheelValue < 0.0f) camera.processScroll(1);
                //else camera.processScroll(-1);
                //scrollWheelValue = currentScrollWheelValue;
            }

            Vector3 originalVector = testDirectionalLight.Direction; // vector to rotate
            Vector3 axis = Vector3.Up; // e.g., Y-axis (0, 1, 0)
            float angleRadians = MathHelper.ToRadians(10 * Time.deltaTime); // 90 degrees

            Matrix rotation = Matrix.CreateFromAxisAngle(axis, angleRadians);
            testDirectionalLight.Direction = Vector3.Transform(originalVector, rotation);


            _input.Process(gameTime);
            counter.Update(gameTime);

            EngineManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            _skybox.Draw(_graphics, _view);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;


            shader.SetUniform("View", _view);
            shader.SetUniform("Projection", _projection);
            shader.SetUniform("viewPos", EngineManager.scene.mainCamera.CameraPosition);
            //testDirectionalLight.SendToShader(shader);
            // TODO: Integrate light objects inheritance from GameObject class
            //shader.SetUniform("pointlight1_position", pointlight_position);
            //testPointLight.SendToShader(shader);
            // TODO: Integrate light objects inheritance from GameObject class
            //shader.SetUniform("spotlight1_position", spotlight_position);
            //testSpotlight.SendToShader(shader);


            //if (useCulling)
            //PerformCulledDraw();
            //else
            EngineManager.Draw(shader, GraphicsDevice, _view, _projection);

            float t = (float)gameTime.TotalGameTime.TotalSeconds;
            _leafSystem.Draw(GraphicsDevice, _view, _projection, t);

            _enviro.Draw(GraphicsDevice, shader);


            //PerformCulledDraw();
            //EngineManager.Draw(shader);
            //Frustum Culling Setup

            /*
            foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        shadowShader.SetUniform("LightViewProj", _world * _lightViewProjection);

                        shadowShader.Effect.CurrentTechnique.Passes[0].Apply();

                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        int primitiveCount = part.PrimitiveCount;
                        int vertexOffset = part.VertexOffset;
                        int startIndex = part.StartIndex;

                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex, primitiveCount);
                    }
                    */

            _text.Begin();
            _text.DrawString(_font, MathF.Ceiling(counter.avgFPS).ToString(), frameraterCounterPosition, Color.Aqua);
            _text.End();


            base.Draw(gameTime);
        }

        private void PerformCulledDraw()
        {
            // Build view-projection frustum
            var viewProjection = _view * _projection;
            var frustum = new BoundingFrustum(viewProjection);

            // Debug wireframe setup if enabled
            var prevRasterizer = GraphicsDevice.RasterizerState;
            if (useDebugWireframe)
            {
                _debugEffect.View = _view;
                _debugEffect.Projection = _projection;
                _debugEffect.World = Matrix.Identity;
                GraphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };
            }

            // Iterate through each GameObject in the scene
            foreach (var go in EngineManager.scene.gameObjects)
            {
                if (go.model == null) continue;

                // Compute world transform and object position
                var world = go.transform.getModelMatrix();
                var position = go.transform.position;


                bool objectNormal = useNormalMap && go.normalMap != null;

                //shader.SetTexture("texture_diffuse1", go.texture);
                //shader.SetUniform("useNormalMap", objectNormal ? 1 : 0);
                //shader.SetTexture("texture_normal1", go.normalMap ?? _normalMap);

                // bind per-object PBR maps (fall back to defaults)
                shader.SetTexture("texture_diffuse1", go.texture);
                shader.SetTexture("texture_normal1", go.normalMap ?? _normalMap);
                shader.SetTexture("texture_roughness1", go.roughnessMap ?? _defaultRoughnessMap);
                shader.SetTexture("texture_ao1", go.aoMap ?? _defaultAOMap);

                // toggles (as ints) for the HLSL if-tests
                shader.SetUniform("useNormalMap", (go.normalMap != null && useNormalMap) ? 1 : 0);
                shader.SetUniform("useRoughnessMap", (go.roughnessMap != null) ? 1 : 0);
                shader.SetUniform("useAOMap", (go.aoMap != null) ? 1 : 0);
                Model modelToDraw = go.model;

                // Cull and draw each mesh of the selected model
                foreach (var mesh in modelToDraw.Meshes)
                {
                    // Transform bounding sphere to world space
                    var sphere = mesh.BoundingSphere.Transform(world);
                    bool visible = frustum.Intersects(sphere);

                    // Draw debug bounding box if enabled
                    if (useDebugWireframe)
                    {
                        var box = BoundingBox.CreateFromSphere(sphere);
                        var corners = box.GetCorners();
                        var lines = new VertexPositionColor[24];
                        Color wireColor = visible ? Color.Green : Color.Red;
                        int idx = 0;
                        int[,] edges =
                        {
                            { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 },
                            { 4, 5 }, { 5, 6 }, { 6, 7 }, { 7, 4 },
                            { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 7 }
                        };
                        for (int e = 0; e < edges.GetLength(0); e++)
                        {
                            var a = corners[edges[e, 0]];
                            var b = corners[edges[e, 1]];
                            lines[idx++] = new VertexPositionColor(a, wireColor);
                            lines[idx++] = new VertexPositionColor(b, wireColor);
                        }

                        foreach (var pass in _debugEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            GraphicsDevice.DrawUserPrimitives(
                                PrimitiveType.LineList,
                                lines,
                                0,
                                edges.GetLength(0));
                        }
                    }

                    // Skip drawing if mesh is outside the frustum
                    if (!visible)
                        continue;

                    // Determine distance from camera to object
                    var distance = Vector3.Distance(
                        EngineManager.scene.mainCamera.CameraPosition,
                        position);

                    // Select appropriate LOD model based on distance

                    if (go.LODModels != null && go.LODModels.Count > 0)
                    {
                        modelToDraw = go.GetLODModel(distance);
                    }

                    // Configure effects and draw mesh
                    foreach (var effect in mesh.Effects)
                    {
                        if (effect is BasicEffect basic)
                        {
                            basic.World = world;
                            basic.View = _view;
                            basic.Projection = _projection;
                            basic.EnableDefaultLighting();
                            basic.TextureEnabled = true;
                            basic.Texture = go.texture;
                        }
                        else
                        {
                            shader.SetUniform("World", world);
                        }
                    }

                    mesh.Draw();
                }
            }

            // Restore original rasterizer state
            GraphicsDevice.RasterizerState = prevRasterizer;
        }
    }
}