using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Components
{
    class DebugMoveComponent : Component
    {

        public bool move = true;

        private const float MoveSpeed = 10f;
        private const float RotationSpeed = 30f;
        private const float GravitySpeed = 5f;

        private int left, right, forward, backward = 0;

        public override void Start()
        {

			EngineManager.InputManager.ActionHeld += OnActionHold;
			EngineManager.InputManager.ActionReleased += OnActionRelease;
		}

        public override void Update()
        {

            
            float hor = right - left;
            float vert = forward - backward;

            var gp = GamePad.GetState(PlayerIndex.One);
            if (gp.IsConnected)
            {
                //hor += gp.ThumbSticks.Left.X;
                //vert += gp.ThumbSticks.Left.Y;
            }

            if (!move || !EngineManager.InputManager.gMode)
            {
                hor = 0f;
                vert = 0f;
            }

            gameObject.transform.rotation +=
                new Vector3(0, -hor, 0) * Time.deltaTime * RotationSpeed;

            gameObject.transform.position += gameObject.transform.Forward * -vert * Time.deltaTime * MoveSpeed;
            gameObject.transform.position += Vector3.Down * Time.deltaTime * GravitySpeed;

        }


        private void OnActionHold(string action)
        {
            switch (action)
            {
                case "Forward": forward = 1; break;
                case "Backward": backward = 1; break;
                case "Left": left = 1; break;
                case "Right": right = 1; break;
            }
        }

		private void OnActionRelease(string action)
		{
			switch (action)
			{
				case "Forward": forward = 0; break;
				case "Backward": backward = 0; break;
				case "Left": left = 0; break;
				case "Right": right = 0; break;
			}
		}
	}
}
