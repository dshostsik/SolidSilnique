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
using System.Reflection.Metadata;

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

        private SpriteFont _font;
        private SpriteBatch _text;

        private Skybox _skybox;


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
        public bool mouseFree { get; private set; } = false;


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
        private LeafParticle _leafSystem2;



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
        public void Set1080p(bool fullscreen = false)
        {
            // 1920×1080 is 1080p
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = fullscreen;
            _graphics.ApplyChanges();

            // If you want to prevent the user from resizing:
            Window.AllowUserResizing = false;
            if (!fullscreen)
                Window.IsBorderless = false;
        }

        public void Set1440p(bool fullscreen = false)
        {
            // 2560×1440 is 1440p
            _graphics.PreferredBackBufferWidth = 2560;
            _graphics.PreferredBackBufferHeight = 1440;
            _graphics.IsFullScreen = fullscreen;
            _graphics.ApplyChanges();

            Window.AllowUserResizing = false;
            if (!fullscreen)
                Window.IsBorderless = false;
        }

        /// <summary>
        /// Add your initialization logic here
        /// </summary>
        protected override void Initialize()
        {
            //DISPLAY SETUP
            // Force 1080p windowed
            Set1080p(fullscreen: false);

            // If you’d rather start in fullscreen 1440p, use:
            // Set1440p(fullscreen: true);
            Window.AllowUserResizing = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.IsFullScreen = false;
            //Window.IsBorderless = true;
            // _graphics.HardwareModeSwitch = true;
            //_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            //_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
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

            sunPosition = new Vector3(256f, 0f, 256f);
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
            EngineManager.graphics = GraphicsDevice;
            EngineManager.shader = shader;
            EngineManager.scene = new ProceduralTest();
            

            _skybox = new Skybox();
            _skybox.Setup(Content, _graphics, GraphicsDevice, _projection);

            

            _input = new Input(this);
            _input.ActionHeld += OnActionHeld;
            _input.MouseMoved += OnMouseMoved;
            _input.ActionPressed += OnActionPressed;   // still available
            _input.MouseClicked += OnMouseClicked;
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
            

            EngineManager.Start();

            // Initialize GPU leaf particles
            _leafSystem = new LeafParticle(maxParticles: (int)2e+3,lifeTime: 40f,gravity: new Vector3(0, 0, 0))
            {
                _game = this
            };
            Texture2D leaftex = Content.Load<Texture2D>("Textures/Dust");
            _leafSystem.LoadContent(GraphicsDevice, Content, leaftex);

            _leafSystem2 = new LeafParticle(maxParticles: (int)2e+3,lifeTime: 20f, gravity: new Vector3(0, -0.2f, 0))
            {
                _game = this
            };
            Texture2D leaftex2 = Content.Load<Texture2D>("Textures/leaf_diffuse");
            _leafSystem2.LoadContent(GraphicsDevice, Content, leaftex2);

        }


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

            //Matrix rotation = Matrix.CreateFromAxisAngle(axis, angleRadians);
            //testDirectionalLight.Direction = Vector3.Transform(originalVector, rotation);


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


            //if (useCulling)
            //PerformCulledDraw();
            //else
            EngineManager.Draw(shadowShader, _view, _projection, manager);

            float t = (float)gameTime.TotalGameTime.TotalSeconds;
            _leafSystem.Draw(GraphicsDevice, _view, _projection, t);
            _leafSystem2.Draw(GraphicsDevice, _view, _projection, t);



            _text.Begin();
            _text.DrawString(_font, MathF.Ceiling(counter.avgFPS).ToString(), frameraterCounterPosition, Color.Aqua);
            _text.End();


            base.Draw(gameTime);
        }

        private void OnActionPressed(string action)
        {
            var cam = EngineManager.scene.mainCamera;
            switch (action)
            {
                case "Forward": cam.move(Camera.directions.FORWARD, Time.deltaTime); break;
                case "Backward": cam.move(Camera.directions.BACKWARD, Time.deltaTime); break;
                case "Left": cam.move(Camera.directions.LEFT, Time.deltaTime); break;
                case "Right": cam.move(Camera.directions.RIGHT, Time.deltaTime); break;
                case "Up": cam.move(Camera.directions.UP, Time.deltaTime); break;
                case "Shoot": cam.cameraComponent.Shoot(); break;
                case "ToggleCulling": EngineManager.useCulling = !EngineManager.useCulling; break;
                case "ToggleWireframe": EngineManager.useWireframe = !EngineManager.useWireframe; break;
                case "ToggleCelShadingOn": EngineManager.celShadingEnabled = true; break;
                case "ToggleCelShadingOff": EngineManager.celShadingEnabled = false; break;
                case "SwitchCamera":
                    var scene = EngineManager.scene;
                    if (scene.TPCamera != null)
                    {
                        var tmp = scene.mainCamera;
                        scene.mainCamera = scene.TPCamera;
                        scene.TPCamera = tmp;
                    }
                    break;
                case "ToggleMouseFree":
                    mouseFree = !mouseFree;
                    break;
            }
        }

        private void OnActionReleased(string action)
        {
            // handle key-up
        }

        private void OnActionHeld(string action)
        {
            var cam = EngineManager.scene.mainCamera;
            float dt = Time.deltaTime;
            switch (action)
            {
                case "Forward": cam.move(Camera.directions.FORWARD, dt); break;
                case "Backward": cam.move(Camera.directions.BACKWARD, dt); break;
                case "Left": cam.move(Camera.directions.LEFT, dt); break;
                case "Right": cam.move(Camera.directions.RIGHT, dt); break;
                case "Up": cam.move(Camera.directions.UP, dt); break;
                    
            }
        }

        private void OnMouseMoved(float dx, float dy)
        {
            if(!mouseFree)
            {
                EngineManager.scene.mainCamera.mouseMovement(dx, dy, Time.deltaTimeMs);
                
            }
            
        }

        void OnMouseClicked(MouseButton a) { }

	}
}