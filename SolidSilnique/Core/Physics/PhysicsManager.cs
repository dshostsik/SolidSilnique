using Microsoft.Xna.Framework;
using SolidSilnique.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Physics
{
	static class PhysicsManager
	{

		public static EnvironmentObject enviro;

		//TODO raycast
		static public GameObject Raycast(Vector3 from, Vector3 direction, float distance) {

			Ray r = new Ray(from, direction);
			GameObject closest = null;
			float closestDistance = float.PositiveInfinity;
			foreach (var GameObject in SphereColliderComponent.instances) { 
				float? a = r.Intersects(GameObject.GetComponent<SphereColliderComponent>().boundingSphere);
				if(a.HasValue)
				{
					if(a < closestDistance)
					{
						closestDistance = a.Value;
						closest = GameObject;
					}
				}
			}
			return closest;
		}

		static public Vector3 SphereToSphereCollision(SphereColliderComponent a, SphereColliderComponent b){

			if (a.gameObject != b.gameObject)
			{
				BoundingSphere other = b.boundingSphere;
				if (a.boundingSphere.Intersects(other))
				{
					Vector3 distVector = a.boundingSphere.Center - other.Center;
					float dist = distVector.Length();
					distVector.Normalize();
					float sepDist = (a.boundingSphere.Radius + other.Radius) - dist;
					Vector3 sepVector = distVector * sepDist;
					return sepVector;
				}
			}
			return Vector3.Zero;
		}

		static public Vector3 SphereToEnviroConstraint(SphereColliderComponent a)
		{

			if (enviro != null)
			{
				if (a.boundingSphere.Center.Y - a.boundingSphere.Radius < enviro.GetHeight(a.boundingSphere.Center)) {
					return (enviro.GetHeight(a.boundingSphere.Center) - a.boundingSphere.Center.Y + a.boundingSphere.Radius) *Vector3.Up;
				}
				
			}
			return Vector3.Zero;
		}

		static public Vector3 SphereToTreeCollision(SphereColliderComponent a, TreeColliderComponent b)
		{
			if (a.gameObject != b.gameObject)
			{
				
				Vector3 otherCenter = new Vector3(b.gameObject.transform.position.X, MathF.Min(a.boundingSphere.Center.Y, b.HeightLimit+b.gameObject.transform.position.Y), b.gameObject.transform.position.Z);
				float distance = Vector3.DistanceSquared(a.boundingSphere.Center, otherCenter);
				if (distance < (a.boundingSphere.Radius + b.Radius) * (a.boundingSphere.Radius + b.Radius))
				{
					//Separate

					Vector3 distVector = a.boundingSphere.Center - otherCenter;
					float dist = distVector.Length();
					distVector.Normalize();
					float sepDist = (a.boundingSphere.Radius + b.Radius) - dist;
					Vector3 sepVector = distVector * sepDist;
					return sepVector;
				}
			}

			return Vector3.Zero;
		}

		static public Vector3 SphereToPlaneCollision(SphereColliderComponent a, PlaneColliderComponent b)
		{

			if (a.gameObject != b.gameObject)
			{
				Plane other = b.plane;
				if (a.boundingSphere.Intersects(other) == PlaneIntersectionType.Intersecting)
				{
					//TO DO fix
					float distance = Vector3.Dot(other.Normal, a.boundingSphere.Center) - other.D;
					float sepDist = (a.boundingSphere.Radius) - distance;
					Vector3 sepVector = other.Normal * sepDist;
					return sepVector;
				}
			}

			return Vector3.Zero;
		}

		//SPHERE TO BOX

		//BOX VARIANTS

		//


	}
}
