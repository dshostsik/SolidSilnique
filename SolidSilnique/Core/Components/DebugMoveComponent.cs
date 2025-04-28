using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Components
{
	class DebugMoveComponent : Component
	{
		public override void Start()
		{

		}

		public override void Update()
		{

			gameObject.transform.position += new Vector3(Time.deltaTime * 10,0,0);

		}
	}
}
