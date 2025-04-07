using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace SolidSilnique
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;

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


        private SpriteBatch _rect;
        private Texture2D _rectTexture;
        private Vector2 _rectPos;
        private Vector2 _rectOrigin;

        private Model _deimos;

        private Camera camera;
        private float lastX, lastY;
        private bool firstMouse;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            IsFixedTimeStep = false;
            
            _graphics.SynchronizeWithVerticalRetrace = true;

            counter = new FrameCounter();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.AllowUserResizing = true;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            // Create camera
            camera = new Camera(new Vector3(0, 0, 5));
            // matrices initialisations
            _world = Matrix.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
            // Resize world matrix
            _world = Matrix.CreateScale(0.05f) * _world;

            //_view = Matrix.CreateLookAt(new Vector3(0, 0, -5), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                1000f
            );

            _whatsAppIconPos = new Vector2(_graphics.PreferredBackBufferWidth * 0.95f,
                _graphics.PreferredBackBufferHeight * 0.01f);
            _textPos = new Vector2(_graphics.PreferredBackBufferWidth * 0.1f,
                _graphics.PreferredBackBufferHeight * 0.05f);
            _rectPos = new Vector2(_graphics.PreferredBackBufferWidth * 0.85f,
                _graphics.PreferredBackBufferHeight * 0.80f);
            frameraterCounterPosition = new Vector2(_graphics.PreferredBackBufferWidth * 0.025f,
                _graphics.PreferredBackBufferHeight * 0.01f);

            lastX = _graphics.PreferredBackBufferWidth / 2;
            lastY = _graphics.PreferredBackBufferHeight / 2;
            firstMouse = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            // Load the model
            _deimos = Content.Load<Model>("deimos");
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
        }

        private void processMouse(GameTime gameTime)
        {
            float mouseX = Mouse.GetState().X;
            float mouseY = Mouse.GetState().Y;

            if (firstMouse)
            {
                lastX = mouseX;
                lastY = mouseY;
                firstMouse = false;
            }

            float xOffset = (mouseX - lastX);
            float yOffset = (lastY - mouseY);

            lastX = mouseX;
            lastY = mouseY;

            // if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            // {
                 camera.mouseMovement(xOffset, yOffset, gameTime.ElapsedGameTime.Milliseconds);
            // }
        }

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

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _view = camera.getViewMatrix();
            // // Rotate object
            _world *= Matrix.CreateRotationY(MathHelper.ToRadians(gameTime.ElapsedGameTime.Milliseconds * 0.01f));

            camera.processScroll(Mouse.GetState().ScrollWheelValue);
            processKeyboard(gameTime);
            processMouse(gameTime);
            counter.Update(gameTime);
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.Aqua);

            _deimos.Draw(_world, _view, _projection);

            _whatsAppIcon.Begin();
            _whatsAppIcon.Draw(_whatsAppIconTexture, _whatsAppIconPos, Color.White);
            _whatsAppIcon.End();

            Vector2 textCenter = _font.MeasureString(gameTime.ElapsedGameTime.Milliseconds.ToString()) / 2;

            _text.Begin();
            _text.DrawString(_font, gameTime.TotalGameTime.Milliseconds.ToString(), _textPos, Color.Black, 0,
                textCenter, 1.0f, SpriteEffects.None, 0.5f);
            _text.End();

            _text.Begin();
            _text.DrawString(_font, MathF.Ceiling(counter.avgFPS).ToString(), frameraterCounterPosition, Color.Black);
            _text.End();

            _rect.Begin();
            _rect.Draw(_rectTexture, _rectPos, null, Color.White, (int)gameTime.TotalGameTime.TotalSeconds * 2,
                _rectOrigin,
                1.0f, SpriteEffects.None, 0.5f);

            //_rect.Draw(_rectTexture, _rectPos, Color.White);
            _rect.End();

            base.Draw(gameTime);
        }
    }
}