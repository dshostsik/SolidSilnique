using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
	class EnvironmentObject
	{

		Vector3[,] meshMap;

		VertexBuffer vertexBuffer;

		VertexPositionNormalTexture[] vertices;

		Texture2D texture;

		

		public void Generate(string mapName, ContentManager content, GraphicsDevice graphics, float cellSize = 1, float maxHeight = 10)
		{

			texture = content.Load<Texture2D>("deimos_texture");

			//Load height map
			Texture2D heightMap = content.Load<Texture2D>( mapName + "/heightMap");
			meshMap = new Vector3[heightMap.Height, heightMap.Width];
			Color[] colors = new Color[heightMap.Width * heightMap.Height];
			heightMap.GetData(colors);


			//Set Vectors in Mesh
			for (int i = 0; i < heightMap.Height; i++)
			{
				for (int j = 0; j < heightMap.Width; j++) {
					meshMap[i, j] = new Vector3(j * cellSize, colors[i * heightMap.Width + j].R/255f * maxHeight, i * cellSize);
				}
			}
			int index = 0;
			//Create VertexBuffer
			vertices = new VertexPositionNormalTexture[(heightMap.Width - 1) * (heightMap.Height - 1) * 6];
			for (int i = 0; i < heightMap.Height - 1; i++)
			{
				for (int j = 0; j < heightMap.Width - 1; j++)
				{
					Vector3 topLeft = meshMap[i, j];
					Vector3 topRight = meshMap[i, j + 1];
					Vector3 bottomLeft = meshMap[i + 1, j];
					Vector3 bottomRight = meshMap[i + 1, j + 1];

					Vector2 UVtopLeft = new Vector2(0, 0);
					Vector2 UVtopRight = new Vector2(1, 0);
					Vector2 UVbottomLeft = new Vector2(0, 1);
					Vector2 UVbottomRight = new Vector2(1, 1);

					Vector3 normal1 = Vector3.Cross(bottomLeft - topLeft, topRight - topLeft);
					normal1.Normalize();
					Vector3 normal2 = Vector3.Cross(bottomRight - bottomLeft, topRight - bottomLeft);
					normal2.Normalize();

					//TRIANGLE 1
					vertices[index++] = new VertexPositionNormalTexture(topLeft, normal1, UVtopLeft);
					vertices[index++] = new VertexPositionNormalTexture(bottomLeft, normal1, UVbottomLeft);
					vertices[index++] = new VertexPositionNormalTexture(topRight, normal1, UVtopRight);

					//TRIANGLE 2
					vertices[index++] = new VertexPositionNormalTexture(topRight, normal2, UVtopRight);
					vertices[index++] = new VertexPositionNormalTexture(bottomLeft, normal2, UVbottomLeft);
					vertices[index++] = new VertexPositionNormalTexture(bottomRight, normal2, UVbottomRight);
				}
			}

			vertexBuffer = new VertexBuffer(
				graphics,
				typeof(VertexPositionNormalTexture),
				vertices.Length,
				BufferUsage.WriteOnly
			);
			vertexBuffer.SetData(vertices);

		}

		public void Draw(GraphicsDevice graphics, Shader shader) {
			graphics.SetVertexBuffer(vertexBuffer);

			shader.SetUniform("texture_diffuse1", texture);
			shader.SetUniform("World", Matrix.CreateTranslation(new Vector3(0, 10, 0))*Matrix.CreateScale(1));


			foreach (var pass in shader.Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Length / 3);
			}
		}
	}

	
}
