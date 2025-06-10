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

        public override void Start()
		{

		}

		public override void Update()
		{

            var kb = Keyboard.GetState();
            float hor = (kb.IsKeyDown(Keys.Left) ? -1f : 0f) + (kb.IsKeyDown(Keys.Right) ? +1f : 0f);
            float vert = (kb.IsKeyDown(Keys.Up) ? +1f : 0f) + (kb.IsKeyDown(Keys.Down) ? -1f : 0f);

            var gp = GamePad.GetState(PlayerIndex.One);
            if (gp.IsConnected)
            {
                hor += gp.ThumbSticks.Left.X;
                vert += gp.ThumbSticks.Left.Y;
            }

            if (!move)
            {
                hor = 0f;
                vert = 0f;
            }

            gameObject.transform.rotation +=
                new Vector3(0, -hor, 0) * Time.deltaTime * RotationSpeed;

            gameObject.transform.position += gameObject.transform.Forward * -vert * Time.deltaTime * MoveSpeed;
            gameObject.transform.position += Vector3.Down * Time.deltaTime * GravitySpeed;

        }
	}
}
