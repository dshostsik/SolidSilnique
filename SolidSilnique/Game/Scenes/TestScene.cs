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
			this.AddChild(go);

			GameObject go2 = new GameObject("Square4");
			go2.transform.position = new Vector3(20, 0, 0);
			go2.transform.scale = new Vector3(0.75f);
			go2.model = loadedModels["deimos"];
			go2.texture = loadedTextures["deimos"];
			go.AddChild(go2);

			GameObject go3 = new GameObject("Square4");
			go3.transform.position = new Vector3(20, 0, 0);
			go3.transform.scale = new Vector3(0.75f);
			go3.model = loadedModels["deimos"];
			go3.texture = loadedTextures["deimos"];
			go2.AddChild(go3);
		}
	}
}
