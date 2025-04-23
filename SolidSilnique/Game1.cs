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

        private SpriteBatch _whatsAppIcon;
        private Texture2D _whatsAppIconTexture;
        private Vector2 _whatsAppIconPos;

        private SpriteFont _font;
        private SpriteBatch _text;
        private Vector2 _textPos;
        private Vector2 textCenter;

        private SpriteBatch _rect;
        private Texture2D _rectTexture;
        private Vector2 _rectPos;
        private Vector2 _rectOrigin;

        private Model _deimos;
        private Texture2D _deimosTexture;

        private Camera camera;
        private bool firstMouse;

        private float lastX;
        private float lastY;

        int scrollWheelValue;
        int currentScrollWheelValue;


        // create bg
        private SpriteBatch background;
        private Texture2D[] frames;
        private int totalFrames;
        private Rectangle screenBounds;

        // TODO: Remove when unnecessary
        // experimental colors;
        private Vector4 dirlight_ambient;
        private Vector4 dirlight_diffuse;
        private Vector4 dirlight_specular;

        private Vector3 spotlight_position;
        private Vector3 pointlight_position;

        // Custom shader
        private Effect customEffect;
        private Effect testEffect;

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

            // Sprite settings
            _whatsAppIconPos = new Vector2(_graphics.PreferredBackBufferWidth * 0.95f,
                _graphics.PreferredBackBufferHeight * 0.01f);
            _textPos = new Vector2(_graphics.PreferredBackBufferWidth * 0.1f,
                _graphics.PreferredBackBufferHeight * 0.05f);
            _rectPos = new Vector2(_graphics.PreferredBackBufferWidth * 0.85f,
                _graphics.PreferredBackBufferHeight * 0.80f);
            frameraterCounterPosition = new Vector2(_graphics.PreferredBackBufferWidth * 0.025f,
                _graphics.PreferredBackBufferHeight * 0.01f);

            firstMouse = true;

            frames = new Texture2D[10];

            for (int i = 0; i < 10; i++)
            {
                frames[i] = Content.Load<Texture2D>("Background/Frame" + (i + 1));
            }

            background = new SpriteBatch(GraphicsDevice);

            totalFrames = frames.Length;

            screenBounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            customEffect = new BasicEffect(GraphicsDevice);

            dirlight_ambient = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
            dirlight_diffuse = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            dirlight_specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            spotlight_position = new Vector3(10.0f, 0.0f, 0.0f);
            pointlight_position = new Vector3(10.0f, 0.0f, 0.0f);

            base.Initialize();
        }

        /// <summary>
        /// Use this.Content to load your game content here
        /// </summary>
        protected override void LoadContent()
        {
            // Load shaders
            customEffect = Content.Load<Effect>("Shaders/CustomShader");
            customEffect.CurrentTechnique = customEffect.Techniques["BasicColorDrawingWithLights"];

            foreach (var parameter in customEffect.Parameters)
            {
                Console.WriteLine($"Parameter: {parameter.Name}, Type: {parameter.ParameterType}");
            }

            // Load the model
            _deimos = Content.Load<Model>("deimos");
            _deimosTexture = Content.Load<Texture2D>("deimos_texture");
            _whatsAppIconTexture = Content.Load<Texture2D>("whatsapp_1384095");

            _whatsAppIcon = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("Megafont");
            _text = new SpriteBatch(GraphicsDevice);

            _rectTexture = new Texture2D(GraphicsDevice, 100, 100);

            var data = new Color[10000];

            for (int i = 0; i < data.Length; i++)
            {
                if (i % 2 == 0)
                {
                    data[i] = Color.Chartreuse;
                }
                else
                {
                    data[i] = Color.Red;
                }
            }

            _rectTexture.SetData(data);

            _rect = new SpriteBatch(GraphicsDevice);

            _rectOrigin = new Vector2(_rectTexture.Width / 2, _rectTexture.Height / 2);

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


            // TODO: Disabled so far because it is irritating
            // background.Begin();
            // background.Draw(frames[(int)(gameTime.TotalGameTime.TotalMilliseconds / counter.avgFPS % totalFrames)], screenBounds, Color.White);
            // background.End();
            try
            {
                foreach (ModelMesh mesh in _deimos.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        BasicEffect effect = part.Effect as BasicEffect;
                        if (effect != null && effect.Texture != null)
                        {
                            customEffect.Parameters["texture_diffuse1"].SetValue(effect.Texture);
                        }

                        part.Effect = customEffect;
                        customEffect.Parameters["World"].SetValue(_world);
                        customEffect.Parameters["View"].SetValue(_view);
                        customEffect.Parameters["Projection"].SetValue(_projection);
                        customEffect.Parameters["viewPos"].SetValue(camera.CameraPosition);
                        customEffect.Parameters["dirlightEnabled"].SetValue(false);
                        customEffect.Parameters["dirlight_direction"].SetValue(Vector3.Zero);
                        customEffect.Parameters["dirlight_ambientColor"].SetValue(dirlight_ambient);
                        customEffect.Parameters["dirlight_diffuseColor"].SetValue(dirlight_diffuse);
                        customEffect.Parameters["dirlight_specularColor"].SetValue(dirlight_specular);
                        
                        customEffect.Parameters["pointlight1Enabled"].SetValue(true);
                        
                        customEffect.Parameters["pointlight1_position"].SetValue(pointlight_position);
                        
                        customEffect.Parameters["pointlight1_ambientColor"].SetValue(dirlight_ambient);
                        customEffect.Parameters["pointlight1_diffuseColor"].SetValue(dirlight_diffuse);
                        customEffect.Parameters["pointlight1_specularColor"].SetValue(dirlight_specular);
                        
                        customEffect.Parameters["pointlight1_linearAttenuation"].SetValue(0.022f);
                        customEffect.Parameters["pointlight1_quadraticAttenuation"].SetValue(0.0019f);
                        customEffect.Parameters["pointlight1_constant"].SetValue(1);
                        
                        customEffect.Parameters["spotlight1Enabled"].SetValue(false);

                        customEffect.Parameters["spotlight1_direction"].SetValue(Vector3.Zero);
                        customEffect.Parameters["spotlight1_position"].SetValue(spotlight_position);

                        customEffect.Parameters["spotlight1_innerCut"].SetValue(MathHelper.ToRadians(12.5f));
                        customEffect.Parameters["spotlight1_outerCut"].SetValue(MathHelper.ToRadians(17.5f));

                        customEffect.Parameters["spotlight1_linearAttenuation"].SetValue(0.045f);
                        customEffect.Parameters["spotlight1_quadraticAttenuation"].SetValue(0.0075f);
                        customEffect.Parameters["spotlight1_constant"].SetValue(1);

                        customEffect.Parameters["spotlight1_ambientColor"].SetValue(dirlight_ambient);
                        customEffect.Parameters["spotlight1_diffuseColor"].SetValue(dirlight_diffuse);
                        customEffect.Parameters["spotlight1_specularColor"].SetValue(dirlight_specular);
                    }

                    mesh.Draw();
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(
                    "Check uniforms!\nIf you have missed any uniforms or they are not used in shader, this NullReferenceException is thrown");
                throw;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(
                    "Check operations and input/output semantics.\nThis may have caused this InvalidOperationException!");
                throw;
            }

            //_deimos.Draw(_world, _view, _projection);

            // TODO: Disabled so far because it is irritating
            // _whatsAppIcon.Begin();
            // _whatsAppIcon.Draw(_whatsAppIconTexture, _whatsAppIconPos, Color.Aquamarine);
            // _whatsAppIcon.End();
            //
            // textCenter = _font.MeasureString(gameTime.ElapsedGameTime.Milliseconds.ToString()) / 2;
            //
            // _text.Begin();
            // _text.DrawString(_font, gameTime.TotalGameTime.Milliseconds.ToString(), _textPos, Color.Aqua, 0,
            //     textCenter, 1.0f, SpriteEffects.None, 0.5f);
            // _text.End();

            _text.Begin();
            _text.DrawString(_font, MathF.Ceiling(counter.avgFPS).ToString(), frameraterCounterPosition, Color.Aqua);
            _text.End();

            // _rect.Begin();
            // _rect.Draw(_rectTexture, _rectPos, null, Color.White, (int)gameTime.TotalGameTime.TotalSeconds * 2,
            //     _rectOrigin,
            //     1.0f, SpriteEffects.None, 0.5f);
            // _rect.End();
            _spriteBatch.Begin();
            _gui.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}