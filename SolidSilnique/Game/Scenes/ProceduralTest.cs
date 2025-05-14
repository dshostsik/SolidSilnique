using SolidSilnique.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SolidSilnique.ProcderuralFoliage;


namespace SolidSilnique.GameContent;

class ProceduralTest : Scene
{
    public Dictionary<string, Model>		loadedModels = new Dictionary<string, Model>();
		public Dictionary<string, Texture2D>	loadedTextures = new Dictionary<string, Texture2D>();
		List<Model>models = new List<Model>();
		List<Texture2D>textures = new List<Texture2D>();
		List<Model> treeModels = new List<Model>();
		List<Texture2D> treeTextures = new List<Texture2D>();
		ContentManager content;
		public ProceduralTest() {

			
		}

		public override void LoadContent(ContentManager Content)
		{
			loadedModels.Add("drzewo", Content.Load<Model>("drzewo2"));
			loadedModels.Add("deimos", Content.Load<Model>("deimos"));
			loadedModels.Add("plane", Content.Load<Model>("plane"));
			loadedModels.Add("cube", Content.Load<Model>("cube"));
			loadedModels.Add("cone", Content.Load<Model>("cone"));
			loadedModels.Add("sphere", Content.Load<Model>("sphere"));

			loadedTextures.Add("deimos", Content.Load<Texture2D>("deimos_texture"));
			loadedTextures.Add("testTex", Content.Load<Texture2D>("testTex"));
			loadedTextures.Add("simpleGreen", Content.Load<Texture2D>("simpleGreen"));
			loadedTextures.Add("gabTex", Content.Load<Texture2D>("Textures/gab_tex"));
			loadedTextures.Add("gabNo", Content.Load<Texture2D>("Textures/gab_no"));
			loadedTextures.Add("gabRo", Content.Load<Texture2D>("Textures/gab_ro"));
			loadedTextures.Add("gabAo", Content.Load<Texture2D>("Textures/gab_ao"));

			loadedTextures.Add("eye", Content.Load<Texture2D>("Textures/eye_tex"));
			
			
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
			textures.Add(Content.Load<Texture2D>("deimos_texture"));
			
			treeModels.Add(Content.Load<Model>("pModels/tree1"));
			treeModels.Add(Content.Load<Model>("pModels/Tree2"));
			treeTextures.Add(Content.Load<Texture2D>("deimos_texture"));
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
			go.AddComponent(new SphereColliderComponent(3f));
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


			GameObject eye1 = new GameObject("eye1");
			eye1.transform.position = new Vector3(-0.25f*2, 0.209f, 0.427f * 2);
			eye1.transform.scale = new Vector3(0.4f);
			eye1.model = loadedModels["sphere"];
			eye1.texture = loadedTextures["eye"];
			gab.AddChild(eye1);

			GameObject eye2 = new GameObject("eye2");
			eye2.transform.position = new Vector3(0.25f*2, 0.209f, 0.427f*2);
			eye2.transform.scale = new Vector3(0.4f);
			eye2.model = loadedModels["sphere"];
			eye2.texture = loadedTextures["eye"];
			gab.AddChild(eye2);

    }
/*
		void AddTree(ProceduralGrass grass,int i,int j)
		{
			
			float radius = 80.0f;
			Vector2 awooga = new Vector2(i*5, j*5);
			Vector2 center = new Vector2(250, 250);
			float len = Vector2.Distance(awooga, center);
			if (grass.computedNoise[i, j] > 180.5f && len > radius)
			{
				GameObject go = new GameObject("Tree");
				float randX = i*5, randZ = j*5;
				
				randX += grass.DisNoise[i,j] / 60f;
				randZ -= grass.DisNoise[i,j] / 60f;
				Random rand = new Random();
				go.transform.position = new Vector3(randX, 0, randZ);
			

            if (Math.Round(grass.ChooseNoise[i, j] / 100) == 2)
				{
					go.model = loadedModels["drzewo"];
                go.AddLOD(loadedModels["drzewo"], 0f);
                go.AddLOD(loadedModels["deimos"], 100f);
                go.AddLOD(null, 200f);
            }
				else if (Math.Round(grass.ChooseNoise[i, j] / 100) == 1)
				{
					go.model = loadedModels["sphere"];
					go.AddComponent(new SphereColliderComponent(1,true));
				}
				else if (Math.Round(grass.ChooseNoise[i, j] / 100) == 0)
				{
					go.model = loadedModels["cone"];
				}
				else
				{
					go.model = loadedModels["cube"];
				}
				go.texture = loadedTextures["deimos"];

				this.AddChild(go);
			}
			

		}
*/
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
	
}