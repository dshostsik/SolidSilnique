using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.GameContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
	static class EngineManager
	{
		public static Scene scene = new TestScene();
		public static Queue<GameObject> renderQueue = [];

		static void Start()
		{

		}

		static void Update(GameTime gameTime) 
		{
			Time.deltaTimeMs = gameTime.ElapsedGameTime.Milliseconds;
			Time.deltaTime = Time.deltaTimeMs/1000.0f;
		}

		public static void Draw(Shader shader)
		{
			scene.Draw();

			while (renderQueue.Count > 0)
			{

				GameObject go = renderQueue.Dequeue(); 
				try
				{
					shader.SetUniform("texture_diffuse1", go.texture);
					shader.SetUniform("World", go.transform.getModelMatrix());

					foreach (ModelMesh mesh in go.model.Meshes)
					{
						foreach (ModelMeshPart part in mesh.MeshParts)
						{
							part.Effect = shader.Effect;

						}

						mesh.Draw();
					}
				}
				catch (NullReferenceException e)
				{
					Console.WriteLine(
						"Check uniforms!\nIf you have missed any uniforms or they are not used in shader, this NullReferenceException is thrown");
					throw;
				}
				catch (UniformNotFoundException u)
				{
					Console.WriteLine(u.Message);
					throw;
				}


				
			}
		}
	}
}
