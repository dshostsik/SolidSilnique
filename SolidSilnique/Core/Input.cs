using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace SolidSilnique.Core
{
    internal class Input
    {

        private readonly Game1 _game;
        private KeyboardState _kbState;
        private MouseState _msState;
        private bool wasBDownLastFrame;
        private bool wasPDownLastFrame;

        public Input(Game1 game)
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

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ProcessKeyboard(dt);
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

        }
        private void ProcessMouse(GameTime gameTime)
        {
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
    }
}
