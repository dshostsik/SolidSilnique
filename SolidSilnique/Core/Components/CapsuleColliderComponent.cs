using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Components
{

	public struct BoundingCapsule
	{
		
	}
	class CapsuleColliderComponent : Component
	{
		static List<GameObject> instances = [];

		public BoundingSphere boundingSphere;
		
		public bool isStatic = false;

		public CapsuleColliderComponent(float radius, bool isStatic = false) {
			boundingSphere = new BoundingSphere();
			boundingSphere.Radius = radius;
			this.isStatic = isStatic;
		}

		public override void Start()
		{
			instances.Add(gameObject);
			boundingSphere.Center = gameObject.transform.position;
		}

		public override void Update()
		{
			boundingSphere.Center = gameObject.transform.position;
			if(isStatic) return;
			CheckCollisionWithSphere();
			CheckCollisionWithPlane();
		}

		public void CheckCollisionWithSphere()
		{
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
		
		public void CheckCollisionWithPlane()
		{
			foreach (var instance in PlaneColliderComponent.instances)
			{

				if (instance != gameObject) {
					Plane other = instance.GetComponent<PlaneColliderComponent>().plane;
					if (boundingSphere.Intersects(other) == PlaneIntersectionType.Intersecting){ 
						//TO DO fix
						float distance = Vector3.Dot(other.Normal, boundingSphere.Center) - other.D;
						float sepDist = (boundingSphere.Radius) - distance;
						Vector3 sepVector = other.Normal * sepDist;
						gameObject.transform.position += sepVector;
						boundingSphere.Center = gameObject.transform.position;
					}
				}
			}
		}
	}
}
