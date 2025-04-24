using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
	class Transform
	{
		//Globals
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;

		//Locals
		public Vector3 localPosition;
		public Vector3 localRotation;
		public Vector3 localScale;

		//gameObject
		public GameObject gameObject;


		public Transform(GameObject gameObject)
		{
			position = new Vector3(0);
			rotation = new Vector3(0);
			scale = new Vector3(1);

			localPosition = position;
			this.gameObject = gameObject;
		}
	}
}
