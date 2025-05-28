using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using static System.Formats.Asn1.AsnWriter;
using System.Net.NetworkInformation;

namespace SolidSilnique.Core
{
    internal class Input
    {

        private readonly Microsoft.Xna.Framework.Game _game;
        private KeyboardState _kbState;
        private MouseState _msState;
        private GamePadState _gpState;
        private bool wasBDownLastFrame;
        private bool wasPDownLastFrame;
        private bool wasF5DownLastFrame = false;
        private bool wasCapDownLastFrame;
        private bool mouseFree = false;

        public Input(Microsoft.Xna.Framework.Game game)
        {
            _game = game;
        }

        /// <summary>
        /// Call this once per frame from Game1.Update(gameTime)
        /// </summary>
        public void Process(GameTime gameTime)
        {
            // 1) Poll keyboard & mouse exactly once
            _kbState = Keyboard.GetState();
            _gpState = GamePad.GetState(PlayerIndex.One);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ProcessKeyboard(dt);
            ProcessGamePad(dt);
            ProcessMouse(gameTime);
        }




        public void ProcessKeyboard(float dt)
        {
            var cam = EngineManager.scene.mainCamera;


            if (_kbState.IsKeyDown(Keys.W)) cam.move(Camera.directions.FORWARD, dt);
            if (_kbState.IsKeyDown(Keys.S)) cam.move(Camera.directions.BACKWARD, dt);
            if (_kbState.IsKeyDown(Keys.A)) cam.move(Camera.directions.LEFT, dt);
            if (_kbState.IsKeyDown(Keys.D)) cam.move(Camera.directions.RIGHT, dt);
            if (_kbState.IsKeyDown(Keys.Space)) cam.move(Camera.directions.UP, dt);


            var kb = Keyboard.GetState();
            

            bool isCapDown = kb.IsKeyDown(Keys.CapsLock);
            if (isCapDown && !wasCapDownLastFrame)
                mouseFree = !mouseFree;

            wasCapDownLastFrame = isCapDown;

            if (_kbState.IsKeyDown(Keys.F)) cam.cameraComponent.Shoot();

            bool isPDown = kb.IsKeyDown(Keys.P);

            if (isPDown && !wasPDownLastFrame)
                EngineManager.useCulling = !EngineManager.useCulling;

            wasPDownLastFrame = isPDown;

            bool isBDown = kb.IsKeyDown(Keys.B);
            if (isBDown && !wasBDownLastFrame)
                EngineManager.useWireframe = !EngineManager.useWireframe;
            wasBDownLastFrame = isBDown;


            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                EngineManager.celShadingEnabled = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                EngineManager.celShadingEnabled = true;
            }

            bool isF5Down = kb.IsKeyDown(Keys.F5);
                    if (isF5Down && !wasF5DownLastFrame)
                        {
                var scene = EngineManager.scene;
                            // only swap if a TPCamera has been set
                            if (scene.TPCamera != null)
                                {
                    var tmp = scene.mainCamera;
                    scene.mainCamera = scene.TPCamera;
                    scene.TPCamera = tmp;
                                }
                        }
            wasF5DownLastFrame = isF5Down;

        }
        private void ProcessMouse(GameTime gameTime)
        {

            if (!_game.IsActive || mouseFree == true)
                return; // Skip mouse processing if window is not active

            int w = _game.Window.ClientBounds.Center.X;
            int h = _game.Window.ClientBounds.Center.Y;
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

        private void ProcessGamePad(float dt)
        {
            var cam = EngineManager.scene.mainCamera;

            if (!_gpState.IsConnected)
                return;

            Vector2 leftThumb = _gpState.ThumbSticks.Left;
            Vector2 rightThumb = _gpState.ThumbSticks.Right;

            // Left stick
            if (Math.Abs(leftThumb.Y) > 0.1f)
                cam.move(leftThumb.Y > 0 ? Camera.directions.FORWARD : Camera.directions.BACKWARD, dt);

            if (Math.Abs(leftThumb.X) > 0.1f)
                cam.move(leftThumb.X > 0 ? Camera.directions.RIGHT : Camera.directions.LEFT, dt);

            // Right stick
            if (Math.Abs(rightThumb.X) > 0.01f || Math.Abs(rightThumb.Y) > 0.01f)
                cam.mouseMovement(-rightThumb.X * 10f, -rightThumb.Y * 8f, (int)(dt * 1000));

            //A for up
            if (_gpState.Buttons.A == ButtonState.Pressed)
                cam.move(Camera.directions.UP, dt);

            if (_gpState.Triggers.Right > 0.1f) cam.cameraComponent.Shoot();
        }
    }
}
