using GUIRESOURCES;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.Core.Components;
using System;

namespace SolidSilnique.GameContent
{
	class TestScene : Scene
	{
		

		public TestScene() {

			
		}

		public override void LoadContent(ContentManager Content)
		{

			loadedTextures.Add("test", Content.Load<Texture2D>("Textures/test_tex"));
            loadedTextures.Add("test_ro", Content.Load<Texture2D>("Textures/test_roughness"));
            loadedTextures.Add("test_ao", Content.Load<Texture2D>("Textures/test_ao"));
            loadedTextures.Add("test_no", Content.Load<Texture2D>("Textures/test_normal"));
            loadedTextures.Add("wood", Content.Load<Texture2D>("Textures/wood_tex"));
            loadedTextures.Add("wood_ro", Content.Load<Texture2D>("Textures/wood_ro"));
            loadedTextures.Add("wood_ao", Content.Load<Texture2D>("Textures/wood_ao"));
            loadedTextures.Add("wood_no", Content.Load<Texture2D>("Textures/wood_no"));

            loadedModels.Add("drzewo", Content.Load<Model>("drzewo2"));
			loadedModels.Add("deimos", Content.Load<Model>("deimos"));
			loadedModels.Add("plane", Content.Load<Model>("plane"));
			loadedTextures.Add("deimos", Content.Load<Texture2D>("deimos_texture"));

            //loadedModels.Add("gun_high", Content.Load<Model>("gun_high"));
            //loadedModels.Add("gun_mid", Content.Load<Model>("gun_mid"));
            //loadedModels.Add("gun_low", Content.Load<Model>("gun_low"));
            //loadedTextures.Add("gun", Content.Load<Texture2D>("deimos_texture"));
        
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
				AddTree();

			}

			for (int i = 0; i < 5; i++)
			{
				AddPlanet();

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
			/*
            //--------------------
            var gun = new GameObject("Gun");
            gun.transform.position = new Vector3(70, 0, -5);
                       // Assign the highest-detail model by default
            gun.model = loadedModels["gun_high"];
            gun.texture = loadedTextures["gun"];

            gun.normalMap = loadedTextures["gun_normal"];
            // Register LOD variants with distance thresholds
            gun.AddLOD(loadedModels["gun_high"], 0f);
            gun.AddLOD(loadedModels["gun_mid"], 30f);
            gun.AddLOD(loadedModels["gun_low"], 50f);


            this.AddChild(gun);


        }
        */
            var test = new GameObject("Square6");
            test.transform.position = new Vector3(70, 20, -5);
            // Assign the highest-detail model by default
            test.model = loadedModels["deimos"];
            test.texture = loadedTextures["test"];

            test.normalMap = loadedTextures["test_no"];
			test.roughnessMap = loadedTextures["test_ro"];
			test.aoMap = loadedTextures["test_ao"];
            this.AddChild(test);

            var test2 = new GameObject("Square6");
            test2.transform.position = new Vector3(70, 20, -15);
            // Assign the highest-detail model by default
            test2.model = loadedModels["deimos"];
            test2.texture = loadedTextures["test"];

            test2.normalMap = loadedTextures["test_no"];

            this.AddChild(test2);

            var test3 = new GameObject("Square6");
            test3.transform.position = new Vector3(70, 20, -25);
            // Assign the highest-detail model by default
            test3.model = loadedModels["deimos"];
            test3.texture = loadedTextures["wood"];
            test3.normalMap = loadedTextures["wood_no"];
            test3.roughnessMap = loadedTextures["wood_ro"];
            test3.roughnessMap = loadedTextures["wood_ao"];
            this.AddChild(test3);

            var test4 = new GameObject("Square6");
            test4.transform.position = new Vector3(70, 20, -35);
            // Assign the highest-detail model by default
            test4.model = loadedModels["deimos"];
            test4.texture = loadedTextures["test"];

            test4.aoMap = loadedTextures["test_ao"];
            
            this.AddChild(test4);

            

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
