using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Components
{
	class SphereColliderComponent : Component
	{
		static List<GameObject> instances = [];

		public BoundingSphere boundingSphere;

		public SphereColliderComponent(float radius) {
			boundingSphere = new BoundingSphere();
			boundingSphere.Radius = radius;
		}

		public override void Start()
		{
			instances.Add(gameObject);
			boundingSphere.Center = gameObject.transform.position;
		}

		public override void Update()
		{
			boundingSphere.Center = gameObject.transform.position;
			foreach (var instance in instances)
			{

				if (instance != gameObject) {
					BoundingSphere other = instance.GetComponent<SphereColliderComponent>().boundingSphere;
					if (boundingSphere.Intersects(other)){ 
						Vector3 distVector = boundingSphere.Center - other.Center;
						float dist = distVector.Length();
						distVector.Normalize();
						float sepDist = (boundingSphere.Radius + other.Radius) - dist;
						Vector3 sepVector = distVector * sepDist;
						gameObject.transform.position += sepVector;
						boundingSphere.Center = gameObject.transform.position;
					}
				}
			}
		}
	}
}
