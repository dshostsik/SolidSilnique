using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SolidSilnique.Core.Components
{

	public struct BoundingCapsule
	{
		float height;
		float radius;

		Vector3 aOffset;
		Vector3 bOffset;


		BoundingCapsule(float height, float radius)
		{
			this.height = height;
			this.radius = radius;
			if(this.height > this.radius*2 ) { 
				this.height = this.radius*2;
			}
			this.aOffset = new Vector3( 0, this.height/2 - this.radius, 0 );
			this.bOffset = new Vector3( 0, -this.height/2 + this.radius, 0 );
		}
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
