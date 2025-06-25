using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Components
{
	public class RhythmBall : Component
	{

		public GameObject target;
		float speed = 20;
		float dec = 0;
		float acc = 3;
		float turnRate = 2f;
		float eps = 0.5f;
		Vector3 prevRot;


		public override void Start()
		{
		}

		public override void Update()
		{
			if (gameObject.model == null) {
				speed = 20;
				turnRate = 2f;
				return;
			}
			
			speed += acc * Time.deltaTime;
			gameObject.transform.position += gameObject.transform.Forward * Time.deltaTime * speed;

			
				prevRot = gameObject.transform.rotation;
				gameObject.transform.LookAt(target.transform.position);

				float t = turnRate * Time.deltaTime;
				Vector3 resultEuler = new Vector3(
					LerpAngle(prevRot.X, gameObject.transform.rotation.X, t),
					LerpAngle(prevRot.Y, gameObject.transform.rotation.Y, t),
					LerpAngle(prevRot.Z, gameObject.transform.rotation.Z, t)
				);
				gameObject.transform.rotation = resultEuler;
				
			turnRate *= 1+Time.deltaTime;
			
			

			if (Vector3.DistanceSquared(gameObject.transform.position, target.transform.position) <= eps * eps) {
				gameObject.model = null;
			}
		}

		public static float LerpAngle(float a, float b, float t)
		{
			float delta = ((b - a + 180f) % 360f + 360f) % 360f - 180f;
			return a + delta * t;
		}
	}
}
