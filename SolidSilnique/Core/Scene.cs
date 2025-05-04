using Microsoft.Xna.Framework.Content;
using SolidSilnique.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
	abstract class Scene
	{
		public string name = "scene";
		public List<GameObject> gameObjects = [];
		public Camera mainCamera;


		public abstract void Setup();
		public abstract void LoadContent(ContentManager Content);


		/// <summary>
		/// Wykonywana na początku programu
		/// </summary>
		public void Start()
		{
			foreach (var child in gameObjects)
			{
				child.Start();
			}

		}

		/// <summary>
		/// Wykonywana w każdej klatce programu
		/// </summary>
		public void Update()
		{


			foreach (var child in gameObjects)
			{
				child.Update();
			}


		}

		/// <summary>
		/// Wykonywana w pętli rysowania
		/// </summary>
		public void Draw()
		{
			foreach (var child in gameObjects)
			{
				child.Draw();
			}

		}


		//SCENE GRAPH

		public void AddChild(GameObject child)
		{
			gameObjects.Add(child);

		}

		public void RemoveChild(GameObject child)
		{
			gameObjects.Remove(child);

		}
	}
}
