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
    public enum MouseButton { Left, Middle, Right }

    class ActionBinding
    {
        public List<Keys> Keys = new();
        public List<Buttons> Buttons = new();
        public List<Func<KeyboardState, GamePadState, bool>> Conditions = new();

        public bool IsPressed(KeyboardState kb, GamePadState gp)
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


            //Console.WriteLine("Mouse position: " + Mouse.GetState().X + " " + Mouse.GetState().Y);
            //Console.WriteLine("Mouse position (using Mouse.GetState().Position): " + Mouse.GetState().Position.X + " " +
            //                  Mouse.GetState().Position.Y);
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

    public class Input
    {
        private readonly Microsoft.Xna.Framework.Game _game;

        private KeyboardState _kbState, _prevKbState;
        private GamePadState _gpState, _prevGpState;
        private MouseState _msState, _prevMsState;
        

        private readonly Dictionary<string, ActionBinding> _bindings = new();
        private readonly Dictionary<string, bool> _prevActionState = new();

        public event Action<string> ActionPressed;
        public event Action<string> ActionReleased;
        public event Action<string> ActionHeld;
        public event Action<MouseButton> MouseClicked;
        public event Action<float, float> MouseMoved;

        public Input(Game1 game)
        {
            _game = game;
            InitializeDefaultBindings();
            
        }
        public Input(ForgeSceneEditor game)
        {
            _game = game;
            InitializeDefaultBindings();
        }

        public void Process(GameTime gameTime)
        {
            // store old states
            _prevKbState = _kbState;
            _prevGpState = _gpState;
            _prevMsState = _msState;

            // poll new
            _kbState = Keyboard.GetState();
            _gpState = GamePad.GetState(PlayerIndex.One);
            _msState = Mouse.GetState();

            EvaluateActions();
            EvaluateHeldActions();
            EvaluateMouseClicks();
            EvaluateMouseMovement();
        }

        private void EvaluateActions()
        {
            foreach (var kv in _bindings)
            {
                string action = kv.Key;
                bool pressed = kv.Value.IsPressed(_kbState, _gpState);
                bool wasPressed = _prevActionState[action];

                if (pressed && !wasPressed) ActionPressed?.Invoke(action);
                if (!pressed && wasPressed) ActionReleased?.Invoke(action);

                _prevActionState[action] = pressed;
            }
        }

        private void EvaluateHeldActions()
        {
            foreach (var kv in _bindings)
            {
                string action = kv.Key;
                if (_bindings[action].IsPressed(_kbState, _gpState))
                    ActionHeld?.Invoke(action);
            }
        }

        private void EvaluateMouseClicks()
        {
            if (_msState.LeftButton == ButtonState.Pressed && _prevMsState.LeftButton == ButtonState.Released)
                MouseClicked?.Invoke(MouseButton.Left);
            if (_msState.RightButton == ButtonState.Pressed && _prevMsState.RightButton == ButtonState.Released)
                MouseClicked?.Invoke(MouseButton.Right);
            if (_msState.MiddleButton == ButtonState.Pressed && _prevMsState.MiddleButton == ButtonState.Released)
                MouseClicked?.Invoke(MouseButton.Middle);
        }

        private void EvaluateMouseMovement()
        {
            var center = _game.Window.ClientBounds.Center;

            var game1 = _game as Game1;

            float dx = _msState.X - center.X;
            float dy = _msState.Y - center.Y;
            if (dx != 0 || dy != 0)
            {
                MouseMoved?.Invoke(dx, dy);
                // recenter for next delta
                if (game1 != null && game1.mouseFree)
                    return;
                Mouse.SetPosition(center.X, center.Y);

            }
        }

        private void InitializeDefaultBindings()
        {
            void Add(string name, ActionBinding bind)
            {
                _bindings[name] = bind;
                _prevActionState[name] = false;
            }

            // Forward = W, UpArrow, left-stick-up
            var forward = new ActionBinding();
            forward.Keys.Add(Keys.W);
            forward.Keys.Add(Keys.Up);
            forward.Conditions.Add((kb, gp) => gp.ThumbSticks.Left.Y > 0.3f);
            Add("Forward", forward);

            // Backward = S, DownArrow, left-stick-down
            var back = new ActionBinding();
            back.Keys.Add(Keys.S);
            back.Keys.Add(Keys.Down);
            back.Conditions.Add((kb, gp) => gp.ThumbSticks.Left.Y < -0.3f);
            Add("Backward", back);

            // Left = A, LeftArrow, left-stick-left
            var left = new ActionBinding();
            left.Keys.Add(Keys.A);
            left.Keys.Add(Keys.Left);
            left.Conditions.Add((kb, gp) => gp.ThumbSticks.Left.X < -0.3f);
            Add("Left", left);

            // Right = D, RightArrow, left-stick-right
            var right = new ActionBinding();
            right.Keys.Add(Keys.D);
            right.Keys.Add(Keys.Right);
            right.Conditions.Add((kb, gp) => gp.ThumbSticks.Left.X > 0.3f);
            Add("Right", right);

            // Up = Space or GamePad A
            var up = new ActionBinding();
            up.Keys.Add(Keys.Space);
            up.Buttons.Add(Buttons.A);
            Add("Up", up);

            // Shoot = F or RightTrigger
            var shoot = new ActionBinding();
            shoot.Keys.Add(Keys.F);
            shoot.Conditions.Add((kb, gp) => gp.Triggers.Right > 0.1f);
            Add("Shoot", shoot);

            // Toggles and camera switch
            Add("ToggleCulling", new ActionBinding { Keys = { Keys.P } });
            Add("ToggleWireframe", new ActionBinding { Keys = { Keys.B } });
            Add("ToggleCelShadingOn", new ActionBinding { Keys = { Keys.F2 } });
            Add("ToggleCelShadingOff", new ActionBinding { Keys = { Keys.F1 } });
            Add("SwitchCamera", new ActionBinding { Keys = { Keys.F5 } });
            Add("ToggleMouseFree", new ActionBinding { Keys = { Keys.CapsLock } });
        }

        /// <summary>Returns whether an action is currently down (for polling if you prefer).</summary>
        public bool IsActionDown(string action) =>
            _bindings.ContainsKey(action) &&
            _bindings[action].IsPressed(_kbState, _gpState);
    } 
}

