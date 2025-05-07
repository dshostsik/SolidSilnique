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
		public Dictionary<string, Model>		loadedModels = new Dictionary<string, Model>();
		public Dictionary<string, Texture2D>	loadedTextures = new Dictionary<string, Texture2D>();

		public TestScene() {

			
		}

		public override void LoadContent(ContentManager Content)
		{
            loadedModels.Add("deimos", Content.Load<Model>("deimos"));
			loadedTextures.Add("deimos", Content.Load<Texture2D>("deimos_texture"));
            
            loadedModels.Add("gun_high", Content.Load<Model>("gun_high"));
            loadedModels.Add("gun_mid", Content.Load<Model>("gun_mid"));
            loadedModels.Add("gun_low", Content.Load<Model>("gun_low"));
            loadedTextures.Add("gun", Content.Load<Texture2D>("Textures/gun_texture"));
            loadedTextures.Add("gun_normal", Content.Load<Texture2D>("Textures/gun_normal"));
			loadedTextures.Add("test", Content.Load<Texture2D>("Textures/test_tex"));
            loadedTextures.Add("test_ro", Content.Load<Texture2D>("Textures/test_roughness"));
            loadedTextures.Add("test_ao", Content.Load<Texture2D>("Textures/test_ao"));
            loadedTextures.Add("test_no", Content.Load<Texture2D>("Textures/test_normal"));

        }

		public override void Setup()
		{
			GameObject go = new GameObject("Camera");
			go.transform.position = new Vector3(0, 0, 0);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			CameraComponent cam = new CameraComponent();
			cam.SetMain();
			go.AddComponent(cam);
			this.AddChild(go);

			go = new GameObject("Square1");
			go.transform.position = new Vector3(10, 0, 10);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			go.AddComponent(new SphereColliderComponent(7));
			this.AddChild(go);

			go = new GameObject("Square2");
			go.transform.position = new Vector3(-10, 0, 10);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			this.AddChild(go);

			go = new GameObject("Square3");
			go.transform.position = new Vector3(-10, 0, -10);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			this.AddChild(go);

			go = new GameObject("Square3");
			go.transform.position = new Vector3(30, 0, 10);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			this.AddChild(go);

			go = new GameObject("Square3");
			go.transform.position = new Vector3(50, 0, 10);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			this.AddChild(go);

			go = new GameObject("Square4");
			go.transform.position = new Vector3(10, 0, -10);
			go.model = loadedModels["deimos"];
			go.texture = loadedTextures["deimos"];
			
			go.AddComponent(new DebugMoveComponent()); //<-- Dodawanie componentów
			go.AddComponent(new SphereColliderComponent(7));

			this.AddChild(go);


			GameObject go2 = new GameObject("Square4");
			go2.transform.position = new Vector3(0, 10, 0);
			go2.transform.scale = new Vector3(0.75f);
			go2.model = loadedModels["deimos"];
			go2.texture = loadedTextures["deimos"];
			go.AddChild(go2);

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

            var test = new GameObject("Square6");
            test.transform.position = new Vector3(70, 20, -5);
            // Assign the highest-detail model by default
            test.model = loadedModels["deimos"];
            test.texture = loadedTextures["test"];

            test.normalMap = loadedTextures["test_no"];
			test.roughnessMap = loadedTextures["test_ro"];
			test.aoMap = loadedTextures["test_ao"];


            this.AddChild(test);

        }
	}
}
