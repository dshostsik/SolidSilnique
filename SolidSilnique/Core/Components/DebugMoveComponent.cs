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
		public override void Start()
		{

		}

		public override void Update()
		{

			float hor = Convert.ToInt32(Keyboard.GetState().IsKeyDown(Keys.Left)) - Convert.ToInt32(Keyboard.GetState().IsKeyDown(Keys.Right));
			float vert = Convert.ToInt32(Keyboard.GetState().IsKeyDown(Keys.Up)) - Convert.ToInt32(Keyboard.GetState().IsKeyDown(Keys.Down));
			if (!move)
			{
				hor = 0;
				vert = 0;
			}

			gameObject.transform.position += new Vector3(hor,-0.5f,vert) * Time.deltaTime * 10;

		}
	}
}
