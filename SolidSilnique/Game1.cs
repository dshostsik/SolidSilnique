using System;
using GUIRESOURCES;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolidSilnique.Core;
using SolidSilnique.GameContent;
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

        private Skybox _skybox;

        
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
        //private Effect customEffect;


        private Shader shader;
        
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

			//Window.AllowUserResizing = true;
			_graphics.GraphicsProfile = GraphicsProfile.HiDef;
			//_graphics.IsFullScreen = true;
			_graphics.HardwareModeSwitch = true;
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.SynchronizeWithVerticalRetrace = true; //VSync
			_graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
			_graphics.ApplyChanges();


            // Create camera
                                          //TODO delete
                                                          //TODO delete
            // matrices initialisations
            _world = Matrix.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.Up);           //TODO delete
            
            // Resize world matrix
            _world = Matrix.CreateScale(1.0f) * _world;

			//TODO delete
			// Sprite settings
			_whatsAppIconPos = new Vector2(_graphics.PreferredBackBufferWidth * 0.95f,
                _graphics.PreferredBackBufferHeight * 0.01f);
            _textPos = new Vector2(_graphics.PreferredBackBufferWidth * 0.1f,
                _graphics.PreferredBackBufferHeight * 0.05f);
            _rectPos = new Vector2(_graphics.PreferredBackBufferWidth * 0.85f,
                _graphics.PreferredBackBufferHeight * 0.80f);
            frameraterCounterPosition = new Vector2(_graphics.PreferredBackBufferWidth * 0.025f,
                _graphics.PreferredBackBufferHeight * 0.01f);

			//TODO delete (shouldn't be here)
			firstMouse = true;

            frames = new Texture2D[10];

            for (int i = 0; i < 10; i++)
            {
                frames[i] = Content.Load<Texture2D>("Background/Frame" + (i + 1));
            }

            background = new SpriteBatch(GraphicsDevice);

            totalFrames = frames.Length;

            screenBounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            
            // Load shaders
            shader = new Shader("Shaders/CustomShader",
                GraphicsDevice,
                this,
                "BasicColorDrawingWithLights");
            
            
            dirlight_ambient = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
            dirlight_diffuse = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            dirlight_specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            spotlight_position = new Vector3(10.0f, 0.0f, 0.0f);
            pointlight_position = new Vector3(10.0f, 0.0f, 0.0f);

			_projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
				GraphicsDevice.Viewport.AspectRatio,
				0.1f,
				100f);

			EngineManager.scene = new TestScene();

            _skybox = new Skybox();
            _skybox.Setup(Content,_graphics,GraphicsDevice,_projection);

			base.Initialize();
        }

        /// <summary>
        /// Use this.Content to load your game content here
        /// </summary>
        protected override void LoadContent()
        {
            // Load the model
            
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

            /*foreach(var child in EngineManager.scene.gameObjects)
            {
                child.texture = _deimosTexture;
                child.model = _deimos;
				foreach (var che in child.children)
				{
					che.texture = _deimosTexture;
					che.model = _deimos;
					foreach (var c in che.children)
					{
						c.texture = _deimosTexture;
						c.model = _deimos;
					}
				}
			}*/
            EngineManager.scene.LoadContent(Content);
            EngineManager.scene.Setup();

			EngineManager.scene.mainCamera.mouseMovement(0, 0, 0);

			EngineManager.Start();
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

			EngineManager.scene.mainCamera.mouseMovement(xOffset, yOffset, gameTime.ElapsedGameTime.Milliseconds);
            Mouse.SetPosition(w, h);
        }

        /// <summary>
        /// Function for defining key bindings
        /// </summary>
        /// <param name="gameTime">Object containing time values</param>
        private void processKeyboard(GameTime gameTime)
        {

            Camera cam = EngineManager.scene.mainCamera;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
				cam.move(Camera.directions.FORWARD, Time.deltaTime);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
				cam.move(Camera.directions.BACKWARD, Time.deltaTime);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
				cam.move(Camera.directions.LEFT, Time.deltaTime);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
				cam.move(Camera.directions.RIGHT, Time.deltaTime);
            }

			if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
			{
				cam.move(Camera.directions.DOWN, Time.deltaTime);
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Space))
			{
				cam.move(Camera.directions.UP, Time.deltaTime);
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
            _view = EngineManager.scene.mainCamera.getViewMatrix(); //TODO Delete

            // Control FOV and perspective settings
            currentScrollWheelValue = Mouse.GetState().ScrollWheelValue;
            if (scrollWheelValue != currentScrollWheelValue)
            {
                //if (scrollWheelValue - currentScrollWheelValue < 0.0f) camera.processScroll(1);
                //else camera.processScroll(-1);
                //scrollWheelValue = currentScrollWheelValue;
            }

            

            processKeyboard(gameTime);
            processMouse(gameTime);
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

			_skybox.Draw(_graphics,_view);

			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

			// TODO: Disabled so far because it is irritating
			// background.Begin();
			// background.Draw(frames[(int)(gameTime.TotalGameTime.TotalMilliseconds / counter.avgFPS % totalFrames)], screenBounds, Color.White);
			// background.End();


			shader.SetUniform("View", _view);
			shader.SetUniform("Projection", _projection);
			shader.SetUniform("viewPos", EngineManager.scene.mainCamera.CameraPosition);
			shader.SetUniform("dirlightEnabled", false);
			shader.SetUniform("dirlight_direction", Vector3.Zero);
			shader.SetUniform("dirlight_ambientColor", dirlight_ambient);
			shader.SetUniform("dirlight_diffuseColor", dirlight_diffuse);
			shader.SetUniform("dirlight_specularColor", dirlight_specular);
			shader.SetUniform("pointlight1Enabled", true);
			shader.SetUniform("pointlight1_position", pointlight_position);
			shader.SetUniform("pointlight1_ambientColor", dirlight_ambient);
			shader.SetUniform("pointlight1_diffuseColor", dirlight_diffuse);
			shader.SetUniform("pointlight1_specularColor", dirlight_specular);
			shader.SetUniform("pointlight1_linearAttenuation", 0.022f);
			shader.SetUniform("pointlight1_quadraticAttenuation", 0.0019f);
			shader.SetUniform("pointlight1_constant", 1);
			shader.SetUniform("spotlight1Enabled", false);
			shader.SetUniform("spotlight1_direction", Vector3.Zero);
			shader.SetUniform("spotlight1_position", spotlight_position);
			shader.SetUniform("spotlight1_innerCut", MathHelper.ToRadians(12.5f));
			shader.SetUniform("spotlight1_outerCut", MathHelper.ToRadians(17.5f));
			shader.SetUniform("spotlight1_linearAttenuation", 0.045f);
			shader.SetUniform("spotlight1_quadraticAttenuation", 0.0075f);
			shader.SetUniform("spotlight1_constant", 1);
			shader.SetUniform("spotlight1_ambientColor", dirlight_ambient);
			shader.SetUniform("spotlight1_diffuseColor", dirlight_diffuse);
			shader.SetUniform("spotlight1_specularColor", dirlight_specular);


			EngineManager.Draw(shader);

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