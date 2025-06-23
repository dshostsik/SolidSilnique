using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Content;

namespace SolidSilnique.Core
{
	class Skybox
	{

		TextureCube textureCube;
		public Effect skyboxEffect;
		VertexBuffer vertexBuffer;
		private SamplerState samplerState;

		public void Setup(ContentManager content, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, Matrix projection)
		{
			textureCube = new TextureCube(graphics.GraphicsDevice, 2048, false, SurfaceFormat.Color);
			Texture2D posX = content.Load<Texture2D>("Skybox/left");
			Texture2D negX = content.Load<Texture2D>("Skybox/right");
			Texture2D posY = content.Load<Texture2D>("Skybox/top");
			Texture2D negY = content.Load<Texture2D>("Skybox/bottom");
			Texture2D posZ = content.Load<Texture2D>("Skybox/back");
			Texture2D negZ = content.Load<Texture2D>("Skybox/front");

			SetCubeFaceData(textureCube, CubeMapFace.PositiveX, posX);
			SetCubeFaceData(textureCube, CubeMapFace.NegativeX, negX);
			SetCubeFaceData(textureCube, CubeMapFace.PositiveY, posY);
			SetCubeFaceData(textureCube, CubeMapFace.NegativeY, negY);
			SetCubeFaceData(textureCube, CubeMapFace.PositiveZ, posZ);
			SetCubeFaceData(textureCube, CubeMapFace.NegativeZ, negZ);

			samplerState = new SamplerState
			{
				Filter = TextureFilter.Linear,
				AddressU = TextureAddressMode.Clamp,
				AddressV = TextureAddressMode.Clamp,
				AddressW = TextureAddressMode.Clamp,

			};

			float size = 10;
			
			skyboxEffect = content.Load<Effect>("Shaders/SkyBoxShader");
			if (skyboxEffect == null)
				throw new Exception("SkyBoxShader effect failed to load.");
			
			Matrix world = Matrix.Identity;
			skyboxEffect.Parameters["Projection"].SetValue(projection);

			//skyboxEffect.Parameters["CubeSampler+CubeTexture"].SetValue(textureCube);
			

			var cubeVertices = new VertexPositionTexture[36];

			// Define the size and texture coordinates
			Vector3 topLeftFront = new Vector3(-2, 2, -2) * size;
			Vector3 topRightFront = new Vector3(2, 2, -2) * size;
			Vector3 bottomLeftFront = new Vector3(-2, -2, -2) * size;
			Vector3 bottomRightFront = new Vector3(2, -2, -2) * size;

			Vector3 topLeftBack = new Vector3(-2, 2, 2) * size;
			Vector3 topRightBack = new Vector3(2, 2, 2) * size;
			Vector3 bottomLeftBack = new Vector3(-2, -2, 2) * size;
			Vector3 bottomRightBack = new Vector3(2, -2, 2) * size;

			Vector2 textureTopLeft = new Vector2(0, 0);
			Vector2 textureTopRight = new Vector2(1, 0);
			Vector2 textureBottomLeft = new Vector2(0, 1);
			Vector2 textureBottomRight = new Vector2(1, 1);


			// Front face
			cubeVertices[0] = new VertexPositionTexture(topLeftFront, textureTopLeft);
			cubeVertices[1] = new VertexPositionTexture(bottomRightFront, textureBottomRight);
			cubeVertices[2] = new VertexPositionTexture(bottomLeftFront, textureBottomLeft);

			cubeVertices[3] = new VertexPositionTexture(topLeftFront, textureTopLeft);
			cubeVertices[4] = new VertexPositionTexture(topRightFront, textureTopRight);
			cubeVertices[5] = new VertexPositionTexture(bottomRightFront, textureBottomRight);

			// Back face
			cubeVertices[6] = new VertexPositionTexture(topLeftBack, textureTopLeft);
			cubeVertices[7] = new VertexPositionTexture(bottomLeftBack, textureBottomLeft);
			cubeVertices[8] = new VertexPositionTexture(bottomRightBack, textureBottomRight);

			cubeVertices[9] = new VertexPositionTexture(topLeftBack, textureTopLeft);
			cubeVertices[10] = new VertexPositionTexture(bottomRightBack, textureBottomRight);
			cubeVertices[11] = new VertexPositionTexture(topRightBack, textureTopRight);

			// Top face
			cubeVertices[12] = new VertexPositionTexture(topLeftFront, textureBottomLeft);
			cubeVertices[13] = new VertexPositionTexture(topRightBack, textureTopRight);
			cubeVertices[14] = new VertexPositionTexture(topRightFront, textureBottomRight);

			cubeVertices[15] = new VertexPositionTexture(topLeftFront, textureBottomLeft);
			cubeVertices[16] = new VertexPositionTexture(topLeftBack, textureTopLeft);
			cubeVertices[17] = new VertexPositionTexture(topRightBack, textureTopRight);

			// Bottom face
			cubeVertices[18] = new VertexPositionTexture(bottomLeftFront, textureTopLeft);
			cubeVertices[19] = new VertexPositionTexture(bottomRightFront, textureTopRight);
			cubeVertices[20] = new VertexPositionTexture(bottomRightBack, textureBottomRight);

			cubeVertices[21] = new VertexPositionTexture(bottomLeftFront, textureTopLeft);
			cubeVertices[22] = new VertexPositionTexture(bottomRightBack, textureBottomRight);
			cubeVertices[23] = new VertexPositionTexture(bottomLeftBack, textureBottomLeft);

			// Left face
			cubeVertices[24] = new VertexPositionTexture(topLeftFront, textureTopRight);
			cubeVertices[25] = new VertexPositionTexture(bottomLeftBack, textureBottomLeft);
			cubeVertices[26] = new VertexPositionTexture(bottomLeftFront, textureBottomRight);

			cubeVertices[27] = new VertexPositionTexture(topLeftFront, textureTopRight);
			cubeVertices[28] = new VertexPositionTexture(topLeftBack, textureTopLeft);
			cubeVertices[29] = new VertexPositionTexture(bottomLeftBack, textureBottomLeft);

			// Right face
			cubeVertices[30] = new VertexPositionTexture(topRightFront, textureTopLeft);
			cubeVertices[31] = new VertexPositionTexture(bottomRightFront, textureBottomLeft);
			cubeVertices[32] = new VertexPositionTexture(bottomRightBack, textureBottomRight);

			cubeVertices[33] = new VertexPositionTexture(topRightFront, textureTopLeft);
			cubeVertices[34] = new VertexPositionTexture(bottomRightBack, textureBottomRight);
			cubeVertices[35] = new VertexPositionTexture(topRightBack, textureTopRight);

			// Create a vertex buffer and upload the cube vertices
			vertexBuffer = new VertexBuffer(
				graphicsDevice,
				typeof(VertexPositionTexture),
				cubeVertices.Length,
				BufferUsage.WriteOnly
			);

			vertexBuffer.SetData(cubeVertices);
  
        }

		public void Draw(GraphicsDeviceManager graphics, Matrix view)
		{
			graphics.GraphicsDevice.SamplerStates[0] = samplerState;
			graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
			
			
			Matrix viewNoTranslation = view;
			viewNoTranslation.Translation = Vector3.Zero;

			skyboxEffect.Parameters["View"].SetValue(viewNoTranslation);
			foreach (var pass in skyboxEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
			}
			
		}

		void SetCubeFaceData(TextureCube cube, CubeMapFace face, Texture2D texture)
		{
			Color[] colorData = new Color[texture.Width * texture.Height];
			texture.GetData(colorData);
			Console.WriteLine($"Setting data for {face}: First Pixel = {colorData[0]}");

			try
			{
				cube.SetData(face, 0, null, colorData, 0, colorData.Length);
				Console.WriteLine($"Successfully set data for face {face}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to set data for face {face}: {ex.Message}");
			}
		}
	}
}
