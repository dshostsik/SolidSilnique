using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace SolidSilnique
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        // Model-View-Projection
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;

        private Model _deimos;

        private Camera camera;
        private float lastX, lastY;
        private bool firstMouse;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.AllowUserResizing = true;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
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


            lastX = Window.ClientBounds.Width / 2;
            lastY = Window.ClientBounds.Height / 2;
            firstMouse = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            // Load the model
            _deimos = Content.Load<Model>("deimos");
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

            float xOffset = (lastX - mouseX);
            float yOffset = (mouseY - lastY);

            lastX = mouseX;
            lastY = mouseY;

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                camera.mouseMovement(xOffset, yOffset, gameTime.ElapsedGameTime.Milliseconds);
            }
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
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.Black);

            _deimos.Draw(_world, _view, _projection);

            base.Draw(gameTime);
        }
    }
}