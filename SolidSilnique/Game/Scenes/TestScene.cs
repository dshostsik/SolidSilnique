using GUIRESOURCES;
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

namespace SolidSilnique.GameContent
{
	class TestScene : Scene
	{
		

		public TestScene() {

			
		}

		public override void LoadContent(ContentManager Content)
		{
			loadedModels.Add("drzewo", Content.Load<Model>("drzewo2"));
			loadedModels.Add("deimos", Content.Load<Model>("deimos"));
			loadedModels.Add("plane", Content.Load<Model>("plane"));
			loadedTextures.Add("deimos", Content.Load<Texture2D>("deimos_texture"));

            loadedModels.Add("gun_high", Content.Load<Model>("gun_high"));
            loadedModels.Add("gun_mid", Content.Load<Model>("gun_mid"));
            loadedModels.Add("gun_low", Content.Load<Model>("gun_low"));
            loadedTextures.Add("gun", Content.Load<Texture2D>("deimos_texture"));
        
			loadedTextures.Add("testTex", Content.Load<Texture2D>("testTex"));
			loadedTextures.Add("simpleGreen", Content.Load<Texture2D>("simpleGreen"));
		}

		public override void Setup()
		{
			GameObject go = new GameObject("Camera");
			go.transform.position = new Vector3(0, 3, 0);
			CameraComponent cam = new CameraComponent();
			cam.SetMain();
			go.AddComponent(cam);
			go.AddComponent(new SphereColliderComponent(0.5f));
			
			this.AddChild(go);

			go = new GameObject("ground");
			go.transform.scale = new Vector3(500, 1, 500);
			go.model = loadedModels["plane"];
			go.texture = loadedTextures["simpleGreen"];
			go.AddComponent(new PlaneColliderComponent(new Vector3(0,1,0), true));
			this.AddChild(go);

			for(int i= 0; i < 150;i++)
			{
				//AddTree();

			}

			for (int i = 0; i < 5; i++)
			{
				//AddPlanet();

			}

			go = new GameObject("Testak");
			go.transform.position = new Vector3(5, 2.5f, -5);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			
			go.AddComponent(new DebugMoveComponent()); //<-- Dodawanie componentów
			go.AddComponent(new SphereColliderComponent(3.5f));

			this.AddChild(go);


			GameObject go2 = new GameObject("Square4");
			go2.transform.position = new Vector3(0, 5, 0);
			go2.transform.scale = new Vector3(0.75f);
			go2.model = loadedModels["deimos"];
			go2.texture = loadedTextures["deimos"];

			
			go.AddChild(go2);

            //--------------------
            var gun = new GameObject("Gun");
            gun.transform.position = new Vector3(70, 0, -5);
                       // Assign the highest-detail model by default
            gun.model = loadedModels["gun_high"];
            gun.texture = loadedTextures["deimos"];
			// Register LOD variants with distance thresholds
			gun.AddLOD(loadedModels["gun_high"], 0f);
            gun.AddLOD(loadedModels["gun_mid"], 30f);
            gun.AddLOD(loadedModels["gun_low"], 50f);


            this.AddChild(gun);
        
			this.Serialize();
		}

		void AddTree()
		{
			Random rand = new Random();
			GameObject go = new GameObject("Tree");
			float randX = 0, randZ = 0;
			while (new Vector3(randX, 0, randZ).Length() < 60) {
				randX = (float)rand.NextDouble() * 500 - 250;
				randZ = (float)rand.NextDouble() * 500 - 250;
			}
			

			go.transform.position = new Vector3(randX, 0, randZ);
			go.model = loadedModels["drzewo"];
			go.texture = loadedTextures["simpleGreen"];

			this.AddChild(go);

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
	}

	
}
