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
            foreach (var k in Keys)
                if (kb.IsKeyDown(k)) return true;
            foreach (var b in Buttons)
                if (gp.IsButtonDown(b)) return true;
            foreach (var cond in Conditions)
                if (cond(kb, gp)) return true;
            return false;
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
        private static readonly Point _fixedCenter = new Point(1920 / 2, 1080 / 2);
        public bool gMode;
        public bool move = true;

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
            // Try to cast to Game1 so we can check its mouseFree flag
            var game1 = _game as Game1;

            if (game1 != null && game1.mouseFree)
                return;

            // Always compute delta relative to window center
            var center = _game.Window.ClientBounds.Center;
            float dx = _msState.X - center.X;
            float dy = _msState.Y - center.Y;

            // Fire the MouseMoved event even if (dx, dy) is (0,0)
            MouseMoved?.Invoke(dx, dy);

            // Recenter cursor every frame so that next frame produces a fresh delta
            Mouse.SetPosition(center.X, center.Y);
        }

        private void EvaluatePadLook()
        {

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
            //forward.Keys.Add(Keys.Up);
            forward.Conditions.Add((kb, gp) => gp.ThumbSticks.Left.Y > 0.3f);
            Add("Forward", forward);

            // Backward = S, DownArrow, left-stick-down
            var back = new ActionBinding();
            back.Keys.Add(Keys.S);
            //back.Keys.Add(Keys.Down);
            back.Conditions.Add((kb, gp) => gp.ThumbSticks.Left.Y < -0.3f);
            Add("Backward", back);

            // Left = A, LeftArrow, left-stick-left
            var left = new ActionBinding();
            left.Keys.Add(Keys.A);
            //left.Keys.Add(Keys.Left);
            left.Conditions.Add((kb, gp) => gp.ThumbSticks.Left.X < -0.3f);
            Add("Left", left);

            // Right = D, RightArrow, left-stick-right
            var right = new ActionBinding();
            right.Keys.Add(Keys.D);
            //right.Keys.Add(Keys.Right);
            right.Conditions.Add((kb, gp) => gp.ThumbSticks.Left.X > 0.3f);
            Add("Right", right);

            // Up = Space or GamePad
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

