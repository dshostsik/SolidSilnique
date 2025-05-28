using Microsoft.Xna.Framework;
using SolidSilnique.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Components
{
	class SphereColliderComponent : Component
	{
		static public List<GameObject> instances = [];

		public BoundingSphere boundingSphere;
		
		public bool isStatic = false;

		public SphereColliderComponent(float radius, bool isStatic = false) {
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
			CheckCollisionWithTree();
			CheckCollisionWithEnviro();
		}

		public void CheckCollisionWithSphere()
		{
			foreach (var instance in instances)
			{
				Vector3 sepVector = PhysicsManager.SphereToSphereCollision(this, instance.GetComponent<SphereColliderComponent>());
				if(sepVector != Vector3.Zero)
				{
					gameObject.transform.position += sepVector;
					boundingSphere.Center = gameObject.transform.position;
				}
			}
		}
		
		public void CheckCollisionWithPlane()
		{
			foreach (var instance in PlaneColliderComponent.instances)
			{

				Vector3 sepVector = PhysicsManager.SphereToPlaneCollision(this, instance.GetComponent<PlaneColliderComponent>());
				if (sepVector != Vector3.Zero)
				{
					gameObject.transform.position += sepVector;
					boundingSphere.Center = gameObject.transform.position;
				}

			}
		}

		public void CheckCollisionWithTree()
		{
			foreach (var instance in TreeColliderComponent.instances)
			{

				Vector3 sepVector = PhysicsManager.SphereToTreeCollision(this, instance.GetComponent<TreeColliderComponent>());
				if (sepVector != Vector3.Zero)
				{
					gameObject.transform.position += sepVector;
					boundingSphere.Center = gameObject.transform.position;
				}
			}
		}

		public void CheckCollisionWithEnviro()
		{
			
				Vector3 sepVector = PhysicsManager.SphereToEnviroConstraint(this);
				if (sepVector != Vector3.Zero)
				{
					gameObject.transform.position += sepVector;
					boundingSphere.Center = gameObject.transform.position;
				}
			
		}
	}
}
