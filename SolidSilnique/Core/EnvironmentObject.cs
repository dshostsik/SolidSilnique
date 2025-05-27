using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{

	public struct LayerMaterial {

		public int id;

		public Texture2D mask;

		public Texture2D diffuse;
		public Texture2D ao;
		public Texture2D normal;
		public Texture2D roughness;

	
	}
	public class EnvironmentObject
	{

		Vector3[,] meshMap;
		Vector3[,] normalMap;

		VertexBuffer vertexBuffer;

		VertexPositionNormalTexture[] vertices;

		Texture2D texture;

		float cellSize = 1;

		LayerMaterial[] layerMaterials;

		public void Generate(string mapName, ContentManager content, float cellSize = 1, float maxHeight = 10, int layers = 1)
		{
			GraphicsDevice graphics = EngineManager.graphics;
			this.cellSize = cellSize;
			texture = content.Load<Texture2D>("deimos_texture");

			//Load height map
			Texture2D heightMap = content.Load<Texture2D>( mapName + "/heightMap");
			meshMap = new Vector3[heightMap.Height, heightMap.Width];
			normalMap = new Vector3[heightMap.Height, heightMap.Width];
			Color[] colors = new Color[heightMap.Width * heightMap.Height];
			heightMap.GetData(colors);


			//Set Vectors in Mesh
			for (int i = 0; i < heightMap.Height; i++)
			{
				for (int j = 0; j < heightMap.Width; j++) {
					meshMap[i, j] = new Vector3(j * cellSize, colors[i * heightMap.Width + j].R/255f * maxHeight, i * cellSize);

				}
			}

			//Calc Normals
			for (int i = 0; i < heightMap.Height-1; i++)
			{
				for (int j = 0; j < heightMap.Width-1; j++)
				{
					Vector3 topLeft = meshMap[i, j];
					Vector3 topRight = meshMap[i, j + 1];
					Vector3 bottomLeft = meshMap[i + 1, j];
					Vector3 bottomRight = meshMap[i + 1, j + 1];


					Vector3 normal1 = Vector3.Cross(bottomLeft - topLeft, topRight - topLeft);
					Vector3 normal2 = Vector3.Cross(bottomRight - bottomLeft, topRight - bottomLeft);

					normalMap[i, j] += normal1;           // topLeft
					normalMap[i + 1, j] += normal1;       // bottomLeft
					normalMap[i, j + 1] += normal1;       // topRight

					normalMap[i, j + 1] += normal2;       // topRight
					normalMap[i + 1, j] += normal2;       // bottomLeft
					normalMap[i + 1, j + 1] += normal2;   // bottomRight
				}
			}

			//Normalize Normals
			for (int i = 0; i < heightMap.Height; i++)
			{
				for (int j = 0; j < heightMap.Width; j++)
				{
					normalMap[i, j].Normalize();
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

					Vector3 nTopLeft = normalMap[i, j];
					Vector3 nTopRight = normalMap[i, j + 1];
					Vector3 nBottomLeft = normalMap[i + 1, j];
					Vector3 nBottomRight = normalMap[i + 1, j + 1];

					//TRIANGLE 1
					vertices[index++] = new VertexPositionNormalTexture(topRight, nTopRight, UVtopRight);
					vertices[index++] = new VertexPositionNormalTexture(bottomLeft, nBottomLeft, UVbottomLeft);
					vertices[index++] = new VertexPositionNormalTexture(topLeft, nTopLeft, UVtopLeft);



					//TRIANGLE 2
					vertices[index++] = new VertexPositionNormalTexture(bottomRight, nBottomRight, UVbottomRight);
					vertices[index++] = new VertexPositionNormalTexture(bottomLeft, nBottomLeft, UVbottomLeft);
					vertices[index++] = new VertexPositionNormalTexture(topRight, nTopRight, UVtopRight);
					
					
				}
			}

			vertexBuffer = new VertexBuffer(
				graphics,
				typeof(VertexPositionNormalTexture),
				vertices.Length,
				BufferUsage.WriteOnly
			);
			vertexBuffer.SetData(vertices);


			//Load layers

			layerMaterials = new LayerMaterial[layers];

			for (int i = 0; i < layers; i++) {
				layerMaterials[i] = new LayerMaterial()
				{
					id = i,
					mask = (i == 0) ? null : content.Load<Texture2D>(mapName + "/layer" + i),
					ao = content.Load<Texture2D>(mapName + "/layer" + i + "/ao"),
					diffuse = content.Load<Texture2D>(mapName + "/layer" + i + "/diffuse"),
					normal = content.Load<Texture2D>(mapName + "/layer" + i + "/normal"),
					roughness = content.Load<Texture2D>(mapName + "/layer" + i + "/roughness")

				};
			}

			PhysicsManager.enviro = this;
		}

		public void Draw() {
			GraphicsDevice graphics = EngineManager.graphics;
			Shader shader = EngineManager.shader;
			graphics.SetVertexBuffer(vertexBuffer);

			

			shader.SetUniform("texture_diffuse1", layerMaterials[0].diffuse);
			shader.SetTexture("texture_normal1", layerMaterials[0].normal);
			shader.SetTexture("texture_roughness1", layerMaterials[0].roughness);
			shader.SetTexture("texture_ao1", layerMaterials[0].ao);

			shader.SetUniform("useNormalMap", 0);
			shader.SetUniform("useRoughnessMap", 1);
			shader.SetUniform("useAOMap", 1);
			shader.SetUniform("useLayering", layerMaterials.Length - 1);

			if (layerMaterials.Length > 0) { 
			for(int i = 1; i < layerMaterials.Length; i++)
				{
					
					shader.SetUniform("layer_diffuse_"+i, layerMaterials[i].diffuse);
					shader.SetTexture("layer_ao_" + i, layerMaterials[i].ao);
					shader.SetTexture("layer_roughness_" + i, layerMaterials[i].roughness);
					shader.SetTexture("layer_mask_"+i, layerMaterials[i].mask);
				}
			
			}
			


			shader.SetUniform("World", Matrix.CreateTranslation(new Vector3(0, 0, 0))*Matrix.CreateScale(1));

			float test = GetHeight(new Vector3(12.3f, 0, 11.1f));

			foreach (var pass in shader.Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Length / 3);
			}
		}

		public float GetHeight(Vector3 point) {

			//Find Quad
			int x = (int)(point.X/cellSize); int z = (int)(point.Z / cellSize); //QUAD TOPLEFT CORNER
			x = Math.Clamp(x, 0, meshMap.GetLength(0) - 1);
			z = Math.Clamp(z, 0, meshMap.GetLength(0) - 1);

			Vector3[] triangle = new Vector3[3]; 

			//FIND TRIANGLE
			Vector3 topLeft = meshMap[z,x];
			if ((topLeft.X - point.X) + (topLeft.Z - point.Z) < cellSize)
			{
				//TRI TOPLEFT
				triangle[0] = topLeft; //TOP LEFT
				triangle[1] = meshMap[z+1, x]; //BOTTOM LEFT
				triangle[2] = meshMap[z, x+1]; //TOP RIGHT
			}
			else {
				//TRI BOTTOMRIGHT
				triangle[0] = meshMap[z, x + 1]; //TOP RIGHT
				triangle[1] = meshMap[z + 1, x]; //BOTTOM LEFT
				triangle[2] = meshMap[z+1, x + 1]; //BOTTOM RIGHT
			}

			//Baricentric Interpolation
			float Area = calcArea(triangle);
			float A0 = calcArea([point, triangle[1], triangle[2]]);
			A0 /= Area;
			float A1 = calcArea([triangle[0], point, triangle[2]]);
			A1 /= Area;
			float A2 = 1 - A1 - A0;

			return A0 * triangle[0].Y + A1 * triangle[1].Y + A2 * triangle[2].Y;
		}

		float calcArea(Vector3[] v)
		{
			float area = 0.5f * Math.Abs(
				(v[1].X - v[0].X) * (v[2].Z - v[0].Z) -
				(v[2].X - v[0].X) * (v[1].Z - v[0].Z)
			);
			return area;
		}
	}

	

	
}
