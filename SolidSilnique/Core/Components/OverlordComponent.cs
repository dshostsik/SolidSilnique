using Microsoft.Xna.Framework;
using SolidSilnique.Core.ArtificialIntelligence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SolidSilnique.Core.Camera;

namespace SolidSilnique.Core.Components
{
	public enum OverlordStates { 
			EXPLORE,
			FIGHT,
			RESULTS_WAIT
	
	}
	public enum FightGrade
	{
		F,
		D,
		C,
		B,
		A,
		S

	}
	public class OverlordComponent : Component
	{
		/* CZYM jest OVERLORD?
		 * Overlord to wszechwładca Gampeplay loopa
		 * 
		 * Tankuj z tym
		 * 
		 * Ma se kamere i spotlighta, on wie co z nimi zrobić :)
		 * 
		 * 
		 */


		public OverlordStates state;
		public static OverlordComponent instance;

		//FIGHT VARIABLES
		Follower enemy;
		float enemyProgress;
		float enemyProgressTarget;
		Vector3 arenaPos;
		FightGrade maxGrade;

		public override void Start()
		{

			instance = this;

			GameObject spotlight = new GameObject("olSpotlight");
			spotlight.AddComponent(new Spotlight(0.05f, 0.01f, 1, Vector3.Down, 0.7f, 0.8f));
			
			EngineManager.lightsManager.Spotlights[0] = spotlight.GetComponent<Spotlight>();
			spotlight.transform.position = new Vector3(180, 40, 730);
			spotlight.GetComponent<Spotlight>().Enabled = 0;
			spotlight.GetComponent<Spotlight>().AmbientColor = EngineManager.lightsManager.DirectionalLight.AmbientColor+new Vector4(0.1f);
			spotlight.GetComponent<Spotlight>().DiffuseColor = EngineManager.lightsManager.DirectionalLight.DiffuseColor+new Vector4(1f);
			this.gameObject.AddChild(spotlight);

			GameObject camera = new GameObject("olCamera");
			camera.AddComponent(new CameraComponent());
			this.gameObject.AddChild(camera);

			
			state = OverlordStates.EXPLORE;
			//EngineManager.lightsManager.DirectionalLight.Enabled = 0;
			EngineManager.lightsManager.Start();
		}

		float rotate = 0;

		public override void Update()
		{
			if (state == OverlordStates.FIGHT) {
				rotate += Time.deltaTime/3f;
				Vector3 cPos = arenaPos + Vector3.Up * 6 + Vector3.Right * MathF.Sin(rotate) * 12 + Vector3.Forward * MathF.Cos(rotate) * 12;
				gameObject.children[1].transform.position = cPos;
				gameObject.children[1].transform.LookAt(arenaPos);
			}
		}

		public void SetFight(float targetPoints, GameObject player, GameObject enemy)
		{
			state = OverlordStates.FIGHT;
			arenaPos = (player.transform.position + enemy.transform.position)/2.0f;
			Vector3 displacementVector = (player.transform.position - arenaPos);
			displacementVector.Normalize();
			player.transform.position = arenaPos + displacementVector*5;
			enemy.transform.position = arenaPos - displacementVector*5;

			player.transform.LookAt(enemy.transform.position);
			player.transform.rotation *= Vector3.Up;

			gameObject.children[0].transform.position = arenaPos + Vector3.Up * 10;
			//gameObject.children[1].transform.position = arenaPos + Vector3.Up * 8;
			
			

			gameObject.children[1].GetComponent<CameraComponent>().SetMain();
			
			EngineManager.lightsManager.DirectionalLight.Enabled = 0;
			EngineManager.lightsManager.Spotlights[0].Enabled = 1;
			EngineManager.lightsManager.Start();
			//Reposition Player
			//Reposition Enemy
			//Set Camera

			//Dim scene - create arena

			//Show UIs

			//Start Rhythm UI


		}

		public void Hit()
		{
			//Dodaj Progrss
			//Shoot Star
			//Camera Shake
			//Updateuj UI
		}

		public void FinishFight()
		{
			//SET Result
			//CALC SCORE
			//CALC GRADE
			//SAVE GRADE
			//SHOW Result
			
		}

		
	}
}
