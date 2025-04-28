using System;
using GUIRESOURCES;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace SolidSilnique
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        private SpriteBatch _spriteBatch;

        private GUI _gui;

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

        private Model _deimos;
        private Texture2D _deimosTexture;

        private Camera camera;
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

        private PointLight testPointLight;
        private Spotlight testSpotlight;

        // Custom shader
        //private Effect customEffect;


        private Shader shader;
        private Shader shadowShader;

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
            //Window.AllowUserResizing = true;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            // TODO
            //_graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = true;
            // TODO
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();

            //Mouse.SetPosition(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y);
            Console.WriteLine("Initial mouse position: " + Mouse.GetState().X + " " + Mouse.GetState().Y);
            Console.WriteLine("Initial mouse position (using Mouse.GetState().Position): " +
                              Mouse.GetState().Position.X + " " + Mouse.GetState().Position.Y);

            Console.WriteLine("Client bounds: " + Window.ClientBounds.Width + "x" + Window.ClientBounds.Height);

            // Create camera
            camera = new Camera(new Vector3(0, 0, 25));
            camera.mouseMovement(0, 0, 0);
            // matrices initialisations
            _world = Matrix.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
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


            dirlight_ambient = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
            dirlight_diffuse = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            dirlight_specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            spotlight_position = new Vector3(-15.0f, 0.0f, 0.0f);
            pointlight_position = new Vector3(10.0f, 0.0f, 0.0f);

            testDirectionalLight = new DirectionalLight(Vector3.Zero);
            testDirectionalLight.AmbientColor = dirlight_ambient;
            testDirectionalLight.DiffuseColor = dirlight_diffuse;
            testDirectionalLight.SpecularColor = dirlight_specular;

            testPointLight = new PointLight( 0.022f, 0.0019f, 1);
            testPointLight.AmbientColor = dirlight_ambient;
            testPointLight.DiffuseColor = dirlight_diffuse;
            testPointLight.SpecularColor = dirlight_specular;

            testSpotlight = new Spotlight( 0.007f, 0.0002f, 1, new Vector3(-10, 0, 0), 5.5f, 7.5f);

            testDirectionalLight.Enabled = false;
            testPointLight.Enabled = false;
            testSpotlight.Enabled = true;
            
            sunPosition = new Vector3(50.0f, 50.0f, 0.0f);
            //testSpotlight.Enabled = false;

            base.Initialize();
        }

        /// <summary>
        /// Use this.Content to load your game content here
        /// </summary>
        protected override void LoadContent()
        {
            // Load the model
            _deimos = Content.Load<Model>("deimos");
            _deimosTexture = Content.Load<Texture2D>("deimos_texture");

            _font = Content.Load<SpriteFont>("Megafont");
            _text = new SpriteBatch(GraphicsDevice);

            _gui = new GUI("GUI/resources/UI.xml", Content);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// Function defining mouse behaviour
        /// </summary>
        /// <param name="gameTime">Object containing time values</param>
        private void processMouse(GameTime gameTime)
        {
            int w = Window.ClientBounds.Center.X;
            int h = Window.ClientBounds.Center.Y;
            float mouseX, mouseY;


            Console.WriteLine("Mouse position: " + Mouse.GetState().X + " " + Mouse.GetState().Y);
            Console.WriteLine("Mouse position (using Mouse.GetState().Position): " + Mouse.GetState().Position.X + " " +
                              Mouse.GetState().Position.Y);
            mouseX = w - Mouse.GetState().X;
            mouseY = Mouse.GetState().Y - h;

            float xOffset = (mouseX);
            float yOffset = (mouseY);

            camera.mouseMovement(xOffset, yOffset, gameTime.ElapsedGameTime.Milliseconds);
            Mouse.SetPosition(w, h);
        }

        /// <summary>
        /// Function for defining key bindings
        /// </summary>
        /// <param name="gameTime">Object containing time values</param>
        private void processKeyboard(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                camera.move(Camera.directions.FORWARD, gameTime.ElapsedGameTime.Milliseconds);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                camera.move(Camera.directions.BACKWARD, gameTime.ElapsedGameTime.Milliseconds);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                camera.move(Camera.directions.LEFT, gameTime.ElapsedGameTime.Milliseconds);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                camera.move(Camera.directions.RIGHT, gameTime.ElapsedGameTime.Milliseconds);
            }
        }

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
            _view = camera.getViewMatrix();

            _lightView = Matrix.CreateLookAt(sunPosition, sunPosition + testDirectionalLight.Direction, Vector3.Up);
            _lightProjection = Matrix.CreateOrthographic(200, 200, 0.1f, 100f);
            _lightViewProjection = _lightView * _lightProjection;

            // Rotate object
            //_world *= Matrix.CreateRotationY(MathHelper.ToRadians(gameTime.ElapsedGameTime.Milliseconds * 0.01f));

            // Control FOV and perspective settings
            currentScrollWheelValue = Mouse.GetState().ScrollWheelValue;
            if (scrollWheelValue != currentScrollWheelValue)
            {
                if (scrollWheelValue - currentScrollWheelValue < 0.0f) camera.processScroll(1);
                else camera.processScroll(-1);
                scrollWheelValue = currentScrollWheelValue;
            }

            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(camera.Zoom),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f);

            processKeyboard(gameTime);
            processMouse(gameTime);
            counter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.Black);
            try
            {
                //GraphicsDevice.SetRenderTarget(shadowMapRenderTarget);
                foreach (ModelMesh mesh in _deimos.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = shader.Effect;
                        shader.SetUniform("texture_diffuse1", _deimosTexture);
                        shader.SetUniform("World", _world);
                        shader.SetUniform("View", _view);
                        shader.SetUniform("Projection", _projection);
                        shader.SetUniform("viewPos", camera.CameraPosition);
                        testDirectionalLight.SendToShader(shader);
                        // TODO: Integrate light objects inheritance from GameObject class
                        shader.SetUniform("pointlight1_position", pointlight_position);
                        testPointLight.SendToShader(shader);
                        // TODO: Integrate light objects inheritance from GameObject class
                        shader.SetUniform("spotlight1_position", spotlight_position);
                        testSpotlight.SendToShader(shader);
                    }

                    mesh.Draw();

                    // src: https://community.monogame.net/t/shadow-mapping-on-monogame/8212/2
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
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(
                    "Check uniforms!\nIf you have missed any uniforms or they are not used in shader, this NullReferenceException is thrown");
                throw;
            }
            catch (UniformNotFoundException u)
            {
                Console.WriteLine(u.Message);
                throw;
            }

            _text.Begin();
            _text.DrawString(_font, MathF.Ceiling(counter.avgFPS).ToString(), frameraterCounterPosition, Color.Aqua);
            _text.End();

            _spriteBatch.Begin();
            _gui.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}