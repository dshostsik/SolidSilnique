using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
	abstract class Scene
	{
		public string name { get; set; } = "scene";
		public List<GameObject> gameObjects { get; set; } = [];

		[JsonIgnore]
		public Camera mainCamera;

		[JsonIgnore]
		public Camera TPCamera;

		[JsonIgnore]
		public Dictionary<string, Model> loadedModels = new Dictionary<string, Model>();

		[JsonIgnore]
		public Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();


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


		//Serialization

		public void Serialize()
		{

			var resolver = new DefaultJsonTypeInfoResolver
			{
				Modifiers =
				{
					typeInfo =>
					{
						if (typeInfo.Type == typeof(Component))
						{
							var derivedTypes = AppDomain.CurrentDomain
								.GetAssemblies()
								.SelectMany(a => a.GetTypes())
								.Where(t => typeof(Component).IsAssignableFrom(t) && !t.IsAbstract);

							var polymorphismOptions = new JsonPolymorphismOptions
							{
								TypeDiscriminatorPropertyName = "$type"
							};
							foreach (var t in derivedTypes)
							{
								polymorphismOptions.DerivedTypes.Add(new JsonDerivedType(t, t.Name.ToLower()));
							}

							typeInfo.PolymorphismOptions = polymorphismOptions;
							};
						}
					}

			};

			var options = new JsonSerializerOptions
			{
				WriteIndented = true, // for human-readable formatting
				TypeInfoResolver = resolver,
				Converters = { new Vector3Converter() } // Add the custom converter

			};

			string json = JsonSerializer.Serialize(this, options);
			File.WriteAllText("scene1.scn", json);


		}
	}
}
