using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Components
{
	class PlaneColliderComponent : Component
	{
		public static List<GameObject> instances = [];

		public Plane plane;
		
		public bool isStatic = false;

		public PlaneColliderComponent(Vector3 normal, bool isStatic) {
			plane = new Plane(Vector3.Normalize(normal), 0);
			this.isStatic = isStatic;
		}

		public override void Start()
		{
			instances.Add(gameObject);
		}

		public override void Update()
		{
			if (isStatic) return;
			checkCollisionWithSphere();
			checkCollisionWithPlane();
		}

		void checkCollisionWithSphere()
		{
			
		}
		
		void checkCollisionWithPlane()
		{
			
		}
	}
}
