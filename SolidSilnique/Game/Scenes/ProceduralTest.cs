using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.Core.ArtificialIntelligence;
using SolidSilnique.Core.Components;
using SolidSilnique.ProcderuralFoliage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SolidSilnique.GameContent;

class ProceduralTest : Scene
{
		
		//PROCEDURAL HELPERS
		List<Model>models = new List<Model>();
		List<Texture2D>textures = new List<Texture2D>();
		List<Model> treeModels = new List<Model>();
		List<Texture2D> treeTextures = new List<Texture2D>();



		
		ContentManager content;
		public ProceduralTest() {

			
		}

		public override void LoadContent(ContentManager Content)
		{
			//loadedModels.Add("drzewo", Content.Load<Model>("drzewo2"));
			loadedModels.Add("deimos", Content.Load<Model>("deimos"));
			loadedModels.Add("plane", Content.Load<Model>("plane"));
			loadedModels.Add("cube", Content.Load<Model>("cube"));
			loadedModels.Add("cone", Content.Load<Model>("cone"));
			loadedModels.Add("sphere", Content.Load<Model>("sphere"));
			loadedModels.Add("levelTest", Content.Load<Model>("level_ground"));
			loadedModels.Add("brzewno1/brzewno", Content.Load<Model>("brzewno1/brzewno"));
			loadedModels.Add("brzewno3/brzewno", Content.Load<Model>("brzewno3/brzewno3"));
			loadedModels.Add("drzewo/drzewo", Content.Load<Model>("drzewo/drzewo"));

			loadedTextures.Add("deimos", Content.Load<Texture2D>("deimos_texture"));
			loadedTextures.Add("testTex", Content.Load<Texture2D>("testTex"));
			loadedTextures.Add("simpleGreen", Content.Load<Texture2D>("simpleGreen"));
			loadedTextures.Add("simpleBlack", Content.Load<Texture2D>("simpleBlack"));
			loadedTextures.Add("gabTex", Content.Load<Texture2D>("Textures/gab_tex"));
			loadedTextures.Add("gabNo", Content.Load<Texture2D>("Textures/gab_no"));
			loadedTextures.Add("gabRo", Content.Load<Texture2D>("Textures/gab_ro"));
			loadedTextures.Add("gabAo", Content.Load<Texture2D>("Textures/gab_ao"));

			loadedTextures.Add("eye", Content.Load<Texture2D>("Textures/eye_tex"));

			loadedTextures.Add("leafTex", Content.Load<Texture2D>("Textures/leaf_diffuse"));
			
			loadedTextures.Add("brzewno1/diffuse", Content.Load<Texture2D>("brzewno1/diffuse"));
			loadedTextures.Add("brzewno1/normal", Content.Load<Texture2D>("brzewno1/normal"));
			loadedTextures.Add("brzewno1/ao", Content.Load<Texture2D>("brzewno1/ao"));
			loadedTextures.Add("brzewno1/glossy", Content.Load<Texture2D>("brzewno1/glossy"));
			
			loadedTextures.Add("brzewno3/diffuse", Content.Load<Texture2D>("brzewno3/diffuse"));
			loadedTextures.Add("brzewno3/normal", Content.Load<Texture2D>("brzewno3/normal"));
			loadedTextures.Add("brzewno3/ao", Content.Load<Texture2D>("brzewno3/ao"));
			loadedTextures.Add("brzewno3/glossy", Content.Load<Texture2D>("brzewno3/glossy"));
			
			loadedTextures.Add("drzewo/diffuse", Content.Load<Texture2D>("drzewo/diffuse"));
			loadedTextures.Add("drzewo/normal", Content.Load<Texture2D>("drzewo/normal"));
			loadedTextures.Add("drzewo/ao", Content.Load<Texture2D>("drzewo/ao"));
			loadedTextures.Add("drzewo/glossy", Content.Load<Texture2D>("drzewo/glossy"));
			
			models.Add(Content.Load<Model>("pModels/Rock1"));
			models.Add(Content.Load<Model>("pModels/Branch"));
			models.Add(Content.Load<Model>("pModels/BushBig"));
			models.Add(Content.Load<Model>("pModels/BushBig"));
			models.Add(Content.Load<Model>("pModels/BushBig"));
			models.Add(Content.Load<Model>("pModels/BushBig"));
			models.Add(Content.Load<Model>("pModels/BushSmall"));
			models.Add(Content.Load<Model>("pModels/BushSmall"));
			models.Add(Content.Load<Model>("pModels/BushSmall"));
			//models.Add(Content.Load<Model>("pModels/Log"));
			models.Add(Content.Load<Model>("pModels/Stump"));
			
			models.Add(Content.Load<Model>("brzewno1/brzewno"));
			models.Add(Content.Load<Model>("brzewno3/brzewno3"));
			textures.Add(loadedTextures["leafTex"]);
			textures.Add(loadedTextures["deimos"]);

			treeModels.Add(Content.Load<Model>("pModels/tree1"));
			treeModels.Add(Content.Load<Model>("pModels/Tree2"));
			treeTextures.Add(Content.Load<Texture2D>("Textures/tree1_diffuse"));
			treeTextures.Add(Content.Load<Texture2D>("Textures/tree2_diffuse"));
			//treeTextures.Add(Content.Load<Texture2D>("Textures/gab_tex"));

			content = Content;





		}

		public override void Setup()
		{


			ProceduralGrass newProc = new ProceduralGrass(models,textures,treeModels,treeTextures,content);
			Task task1 = Task.Run(() => newProc.precomputeNoise());
			
			GameObject go = new GameObject("Camera");
			go.transform.position = new Vector3(250, 3f, 250);
			CameraComponent cam = new CameraComponent();
			cam.SetMain();
			go.AddComponent(cam);
			//go.AddComponent(new SphereColliderComponent(3f));
			this.AddChild(go);

			go = new GameObject("ground");
			go.transform.position = new Vector3(250, 0, 250);
			go.transform.scale = new Vector3(500, 1, 500);
			go.model = loadedModels["plane"];
			go.texture = loadedTextures["simpleGreen"];
			go.AddComponent(new PlaneColliderComponent(new Vector3(0,1,0), true));
			this.AddChild(go);
			Task.WhenAll(task1).Wait(); //:O
			
			newProc.GenerateObjects();
			List<GameObject> goList = newProc.createdObjects;

			for (int a = 0; a < goList.Count; a++)
			{
				this.AddChild(goList[a]);
			}
			

			for (int i = 0; i < 5; i++)
			{
				AddPlanet();

			}

			GameObject goTest = new GameObject("Deimos");


			goTest.transform.position = new Vector3(150, 2.5f, 150);
			goTest.model = loadedModels["levelTest"];
			goTest.texture = loadedTextures["deimos"];
			goTest.AddComponent(new SphereColliderComponent(3.5f, true));


			this.AddChild(goTest);

		go = new GameObject("Testak");
			go.transform.position = new Vector3(5, 2.5f, -5);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			

        //go.AddComponent(new DebugMoveComponent()); //<-- Dodawanie componentów
        go.AddComponent(new SphereColliderComponent(3.5f));

			this.AddChild(go);


			GameObject go2 = new GameObject("Square4");
			go2.transform.position = new Vector3(0, 5, 0);
			go2.transform.scale = new Vector3(0.75f);
			go2.model = loadedModels["deimos"];
			go2.texture = loadedTextures["deimos"];
			go.AddChild(go2);

			GameObject gab = new GameObject("gab");
			gab.transform.position = new Vector3(250, 15, 220);

            gab.transform.scale = new Vector3(1f);
			gab.model = loadedModels["cube"];
			gab.texture = loadedTextures["gabTex"];
			//gab.normalMap = loadedTextures["gabNo"];
			//gab.roughnessMap = loadedTextures["gabRo"];
			//gab.aoMap = loadedTextures["gabAo"];
			gab.AddComponent(new DebugMoveComponent());
			gab.AddComponent(new SphereColliderComponent(1));
			

			this.AddChild(gab);
			GameObject TPcam = new GameObject("cam");
			var tpcCamComp = new CameraComponent();
			TPcam.AddComponent(tpcCamComp);
			TPcam.transform.position = new Vector3(0,5, -10);
			this.TPCamera = new Camera(tpcCamComp);
			
			gab.AddChild(TPcam);

			GameObject eye1 = new GameObject("eye1");
			eye1.transform.position = new Vector3(-0.25f*2, 0.209f, 0.427f * 2);
			eye1.transform.scale = new Vector3(0.4f);
			eye1.model = loadedModels["sphere"];
			eye1.texture = loadedTextures["eye"];
			gab.AddChild(eye1);

			GameObject pupil1 = new GameObject("pupil1");
				pupil1.transform.position = new Vector3(0, 0, 0.427f * 2);
				pupil1.transform.scale = new Vector3(0.4f,0.4f,0.2f);
				pupil1.model = loadedModels["sphere"];
				pupil1.texture = loadedTextures["simpleBlack"];
				eye1.AddChild(pupil1);

		GameObject brow1 = new GameObject("brow1");
		brow1.transform.position = new Vector3(-0.25f * 2, 0.5f, 0.427f * 2);
		brow1.transform.scale = new Vector3(0.45f, 0.2f,0.4f);
		brow1.transform.rotation = new Vector3(0f,0,-20f);
		brow1.model = loadedModels["cube"];
		brow1.texture = loadedTextures["simpleBlack"];
		gab.AddChild(brow1);

		GameObject eye2 = new GameObject("eye2");
			eye2.transform.position = new Vector3(0.25f*2, 0.209f, 0.427f*2);
			eye2.transform.scale = new Vector3(0.4f);
			eye2.model = loadedModels["sphere"];
			eye2.texture = loadedTextures["eye"];
			gab.AddChild(eye2);

		GameObject pupil2 = new GameObject("pupil1");
		pupil2.transform.position = new Vector3(0, 0, 0.427f * 2);
		pupil2.transform.scale = new Vector3(0.4f, 0.4f, 0.2f);
		pupil2.model = loadedModels["sphere"];
		pupil2.texture = loadedTextures["simpleBlack"];
		eye2.AddChild(pupil2);

		GameObject brow2 = new GameObject("brow1");
		brow2.transform.position = new Vector3(0.25f * 2, 0.5f, 0.427f * 2);
		brow2.transform.scale = new Vector3(0.45f, 0.2f, 0.4f);
		brow2.transform.rotation = new Vector3(0f, 0, 20f);
		brow2.model = loadedModels["cube"];
		brow2.texture = loadedTextures["simpleBlack"];
		gab.AddChild(brow2);

		GameObject prevGeb = gab;
			for (int i = 0; i < 10; i++)
			{
				GameObject gogus = CreateGebus(new Vector3(150 + i*2, 2, 150 + i*2));
				gogus.GetComponent<Follower>().Target = prevGeb;
				if (i == 0) gogus.GetComponent<Follower>().SocialDistanceMultiplier = 4.0f;
				this.AddChild(gogus);
				prevGeb = gogus;
			}
			

			

		}

		void AddPlanet()
		{
			Random rand = new Random();
			GameObject go = new GameObject("Deimos");
			float randX = 0, randZ = 0;
			while (new Vector3(randX, 0, randZ).Length() < 5)
			{
				randX = (float)rand.NextDouble() * 50 - 25;
				randZ = (float)rand.NextDouble() * 50 - 25;
			}


			go.transform.position = new Vector3(randX, 2.5f, randZ);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			go.AddComponent(new SphereColliderComponent(3.5f,true));
			

			this.AddChild(go);

		}
		
		GameObject CreateGebus(Vector3 pos)
		{
			GameObject go = new GameObject("Gebus");

			go.transform.position = pos;
			go.transform.scale = new Vector3(0.75f);
			// go.model = loadedModels["sphere"];
			// go.texture = loadedTextures["gabTex"];
			go.model = loadedModels["drzewo/drzewo"];
			go.texture = loadedTextures["drzewo/diffuse"];
			go.normalMap = loadedTextures["drzewo/normal"];
			go.aoMap = loadedTextures["drzewo/ao"];
			go.roughnessMap = loadedTextures["drzewo/glossy"];
			go.AddComponent(new SphereColliderComponent(0.75f,false));
			go.AddComponent(new DebugMoveComponent());
			go.GetComponent<DebugMoveComponent>().move = false;
			go.AddComponent(new Follower(go, 2f));

			return go;

		}
	
}