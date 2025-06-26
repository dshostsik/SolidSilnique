using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core.ArtificialIntelligence;
using System;
using System.Linq;
using GUIRESOURCES;
using System.Collections.Generic;

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
		GameObject enemy;
		GameObject player;
		public BossRhythymUI rhythymUI;
		public GUI currentGui;
		float enemyProgress;
		float enemyProgressTarget;
		Vector3 arenaPos;
		FightGrade maxGrade;
		int ballPoolCount = 30;
		int ballPoolIndex = 0;
		public float bossMult = 1;
		Model ballModel;

        private int _initialNotesCount;
        private int _finalScore;
        private float _finalAccuracySum;
        private float _finalAccuracyAvg;
        private FightGrade _finalGrade;
        private bool _showResults = false;
        private Dictionary<FightGrade, Texture2D> _gradeTextures;

        public bool ShowResults => _showResults;
        public int FinalScore => _finalScore;
        public float FinalAccuracyAvg => _finalAccuracyAvg;
        public FightGrade FinalGrade => _finalGrade;

        public IReadOnlyDictionary<FightGrade, Texture2D> GradeTextures
            => _gradeTextures;

        public override void Start()
		{

			instance = this;
			currentGui.texts[1].visible = false;
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
			ballModel = EngineManager.scene.loadedModels["sphere"];

			GameObject ballsPool = new GameObject("olBallsPool");
			for (int i = 0; i < ballPoolCount; i++) {
				GameObject ball = new GameObject("olBall"+i);
				ball.AddComponent(new RhythmBall());
				ball.texture = EngineManager.scene.loadedTextures["eye"];
				ball.albedo = new Color(1, 1, 0.4f) ;
				ball.emissive = new Color(1, 1, 0f) ;
				ball.transform.scale = new Vector3(0.15f);
				ballsPool.AddChild(ball);
			}
			this.gameObject.AddChild(ballsPool);
			
			state = OverlordStates.EXPLORE;
			//EngineManager.lightsManager.DirectionalLight.Enabled = 0;
			EngineManager.lightsManager.Start();
			currentGui.progressBars[1].visible = false;
			currentGui.images[2].visible = false;

            _gradeTextures = Enum.GetValues(typeof(FightGrade))
                        .Cast<FightGrade>()
                        .ToDictionary(
                            g => g,
                            g => EngineManager.Content
                                      .Load<Texture2D>($"Grades/{g}"));

        }

		float rotate = 0;
		Vector3 cShake = Vector3.Zero;
		Vector3 cShakeActual = Vector3.Zero;

		public override void Update()
		{
			if (state == OverlordStates.FIGHT) {
				rotate += Time.deltaTime/3f;

				cShakeActual = Vector3.Lerp(cShakeActual, cShake, Time.deltaTime * 10);
				Vector3 cPos = arenaPos
					+ Vector3.Up * (6f + cShakeActual.Y)
					+ Vector3.Right * (MathF.Sin(rotate) * (12f + cShakeActual.Z) * bossMult)
					+ Vector3.Forward * (MathF.Cos(rotate) * (12f + cShakeActual.Z) * bossMult);

				cShake = Vector3.Lerp(cShake, Vector3.Zero, Time.deltaTime * 10);
				gameObject.children[1].transform.position = cPos;
				gameObject.children[1].transform.LookAt(arenaPos);
				currentGui.texts[3].text = enemyProgress.ToString() +" / "+ enemyProgressTarget.ToString() ;

			}
		}

		public void SetFight(BossRhythymUI rhythmUi, float targetPoints, GameObject player, GameObject enemy)
		{
			enemyProgress = 0;
			state = OverlordStates.FIGHT;
			rhythmUi.hit += Hit;
			rhythmUi.combo = 0;
			rhythymUI = rhythmUi;
			enemyProgressTarget = (rhythmUi.loadedNotes.Count()) * 30 * 0.6f;
			this.enemy = enemy;
			this.player = player;
			currentGui.progressBars[1].visible = true;
			currentGui.images[2].visible = true;
			currentGui.progressBars[1].progress = (enemyProgress / enemyProgressTarget) * 100;
			currentGui.texts[1].visible = true;

			arenaPos = (player.transform.position + enemy.transform.position)/2.0f;
			Vector3 displacementVector = (player.transform.position - arenaPos);
			displacementVector.Normalize();
			player.transform.position = arenaPos + displacementVector*5*bossMult;
			enemy.transform.position = arenaPos - displacementVector*5 * bossMult;

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
            _initialNotesCount = (int)(targetPoints / 30f + 0.5f);
            enemyProgress = 0;

        }

		private void Hit(object sender, BossRhythymUI.NoteHitEventArgs e)
		{
			float comboMod = 1+MathF.Min(2,MathF.Max(e.Combo - 1, 0) * 0.05f);
			if (e.Accuracy <= 0.14f && e.Accuracy > 0.8f)
			{
				enemyProgress += 10;// * comboMod; //Great
			}
			else if (e.Accuracy <= 0.8f) {
				enemyProgress += 30;// * comboMod; //Perfect
			}
			currentGui.progressBars[1].progress = MathF.Min(100,(enemyProgress/enemyProgressTarget) * 100);
			//enemy.children[0].emissive = new Color(1, 1, 0) * MathF.Min(1,(enemyProgress / enemyProgressTarget));
			cShake += -Vector3.Forward * comboMod;
			GameObject usedBall = this.gameObject.children[2].children[ballPoolIndex++];
			if (ballPoolIndex >= ballPoolCount) {
				ballPoolIndex = 0;
			}
			usedBall.model = null;
			usedBall.Update();
			usedBall.model = ballModel;
			usedBall.GetComponent<RhythmBall>().target = enemy;
			usedBall.transform.position = player.transform.position;
			
			switch (e.NoteType) {
				case 0: //UP
					usedBall.transform.LookAt(player.transform.getModelMatrix().Up + player.transform.position);
					break;
				case 1: //LEFT
					usedBall.transform.LookAt(player.transform.getModelMatrix().Left + player.transform.position);
					break;


				case 2: //DOWN
					usedBall.transform.LookAt(player.transform.getModelMatrix().Forward + player.transform.position);
					break;
				case 3: //RIGHT
					usedBall.transform.LookAt(player.transform.getModelMatrix().Right + player.transform.position);
					break;
				

			}
			
		}

		

		public void FinishFight(CameraComponent camToSet)
		{
			state = OverlordStates.EXPLORE;
			currentGui.progressBars[1].visible = false;
			currentGui.images[2].visible = false;
			currentGui.texts[1].visible = false;

			rhythymUI.hit -= Hit;








			camToSet.SetMain();

			EngineManager.lightsManager.DirectionalLight.Enabled = 1;
			EngineManager.lightsManager.Spotlights[0].Enabled = 0;
			EngineManager.lightsManager.Start();

            _finalScore = (int)enemyProgress;
           // _finalAccuracySum = rhythmUI.ReturnScoresAndAccuracy();

            //_finalAccuracyAvg = _initialNotesCount > 0
            //                  ? _finalAccuracySum / _initialNotesCount
            //                  : 0f;

            // performance ratio
            float perf = enemyProgress / enemyProgressTarget;
            _finalGrade = perf >= 0.95f ? FightGrade.S
                        : perf >= 0.85f ? FightGrade.A
                        : perf >= 0.70f ? FightGrade.B
                        : perf >= 0.50f ? FightGrade.C
                        : perf >= 0.30f ? FightGrade.D
                        : FightGrade.F;

            _showResults = true;      
            //state = OverlordStates.RESULTS_WAIT; 

            //SET Result
            //CALC SCORE
            //CALC GRADE
            //SAVE GRADE
            //SHOW Result

        }

        

        public void SetGradeTextures(Dictionary<FightGrade, Texture2D> textures)
        {
            _gradeTextures = textures;
        }


    }
}
