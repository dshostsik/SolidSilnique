using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.GameContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUIRESOURCES;
using SolidSilnique.Core.Diagnostics;
using System.Reflection;

namespace SolidSilnique.Core
{

    
    static class EngineManager
    {
        public static Scene scene = null;
        
        public static Queue<GameObject> renderQueue = [];
		public static Dictionary<GameObject, VertexBufferBindingGroup> InstancesQueue = []; //representative, InstanceBuffer
        public static Queue<Tuple<Texture2D,Vector2,Color>> renderQueueUI = [];

        public static bool celShadingEnabled = false;
        public static GUI currentGui;

        //Debug flags
        public static bool useCulling = true;
        public static bool useWireframe = false;

        //Default maps
        public static Texture2D normalMap;
        public static Texture2D defaultRoughnessMap;
        public static Texture2D defaultAOMap;

        public static BasicEffect wireframeEffect;
        public static GraphicsDevice graphics;
        public static Shader shader;

        public static void Start()
        {
            scene.Start();
            GenerateInstanceData();
        }

        public static void Update(GameTime gameTime)
        {
            Time.deltaTimeMs = gameTime.ElapsedGameTime.Milliseconds;
            Time.deltaTime = Time.deltaTimeMs / 1000.0f;

            scene.Update();
        }

        public static void Draw(Matrix view, Matrix projection)
        {
            scene.Draw();

			var viewProjection = view * projection;
			var frustum = new BoundingFrustum(viewProjection);

			while (renderQueue.Count > 0)
            {
                GameObject go = renderQueue.Dequeue();

                //FRUSTUM CULLING
                

                var position = go.transform.position;

                // Determine distance from camera to object
                var distance = Vector3.Distance(
                    EngineManager.scene.mainCamera.CameraPosition,
                    position);

                // Select appropriate LOD model based on distance

                if (go.LODModels != null && go.LODModels.Count > 0)
                {
                    go.model = go.GetLODModel(distance);
                }


                if (go.model == null)
                {
                    continue;
                }

                try
                {
                    Matrix model = go.transform.getModelMatrix();

                    setMaterial(go);


					shader.SetUniform("World", model);
                    Matrix modelTransInv = Matrix.Transpose(Matrix.Invert(go.transform.getModelMatrix()));
                    shader.SetUniform("WorldTransInv", modelTransInv);


                    foreach (ModelMesh mesh in go.model.Meshes)
                    {
                        if (useCulling)
                        {
                            var sphere = mesh.BoundingSphere.Transform(go.transform.getModelMatrix());
                            bool visible = frustum.Intersects(sphere);

                            if (useWireframe)
                            {
                                var prevRasterizer = graphics.RasterizerState;


                                wireframeEffect.View = view;
                                wireframeEffect.Projection = projection;
                                wireframeEffect.World = Matrix.Identity;
                                graphics.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };


                                var box = BoundingBox.CreateFromSphere(sphere);
                                var corners = box.GetCorners();
                                var lines = new VertexPositionColor[24];
                                Color wireColor = visible ? Color.Green : Color.Red;
                                int idx = 0;
                                int[,] edges =
                                {
                                    { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 },
                                    { 4, 5 }, { 5, 6 }, { 6, 7 }, { 7, 4 },
                                    { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 7 }
                                };
                                for (int e = 0; e < edges.GetLength(0); e++)
                                {
                                    var a = corners[edges[e, 0]];
                                    var b = corners[edges[e, 1]];
                                    lines[idx++] = new VertexPositionColor(a, wireColor);
                                    lines[idx++] = new VertexPositionColor(b, wireColor);
                                }

                                foreach (var pass in wireframeEffect.CurrentTechnique.Passes)
                                {
                                    pass.Apply();
                                    graphics.DrawUserPrimitives(
                                        PrimitiveType.LineList,
                                        lines,
                                        0,
                                        edges.GetLength(0));
                                }

                                graphics.RasterizerState = prevRasterizer;
                            }

                            if (!visible)
                                continue;
                        }

                        for (int i = 0; i < mesh.MeshParts.Count; i++)
                        {
                            var part = mesh.MeshParts[i];
                            part.Effect = shader.Effect;
                            if (celShadingEnabled)
                            {
                                part.Effect.CurrentTechnique = shader.Effect.Techniques["CelShadingOutline"];
                                graphics.RasterizerState = RasterizerState.CullClockwise;
                                mesh.Draw();

                                part.Effect.CurrentTechnique = shader.Effect.Techniques["CelShading"];
                                graphics.RasterizerState = RasterizerState.CullCounterClockwise;
                                mesh.Draw();
                            }
                            else
                            {
                                part.Effect.CurrentTechnique = shader.Effect.Techniques["BasicColorDrawingWithLights"];
                                mesh.Draw();
                            }
                        }
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
            if(scene.environmentObject != null)
            {
                scene.environmentObject.Draw(frustum);
            }
			DrawInstanceData();

            SpriteBatch UiRenderer = new SpriteBatch(graphics);
            UiRenderer.Begin();
            while(renderQueueUI.Count > 0)
            {
                Tuple<Texture2D,Vector2,Color> element = renderQueueUI.Dequeue();
                
                UiRenderer.Draw(element.Item1, element.Item2, element.Item3);
                
            }
            
            
            //Rednder Mateuszkowe GUI :) using the same SpriteBatch :) 
            if (currentGui != null)
            {
                currentGui.Draw(UiRenderer);
            }

            
            UiRenderer.End();
        }
    
		






		//INSTANCING

		struct InstanceMatrix : IVertexType
		{
			public Vector4 Row1;
			public Vector4 Row2;
			public Vector4 Row3;
			public Vector4 Row4;

			public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 10),
				new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 11),
				new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 12),
				new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 13)
			);

			VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

			public InstanceMatrix(Matrix matrix)
			{
				Row1 = new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14);
				Row2 = new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24);
				Row3 = new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34);
				Row4 = new Vector4(matrix.M41, matrix.M42, matrix.M43, matrix.M44);
			}
		}

		public struct VertexBufferBindingGroup
		{
            public VertexBufferBinding[] vertexBufferBindings = new VertexBufferBinding[2];
			public int InstanceCount;

            public VertexBufferBindingGroup() { }
		}

        static void setMaterial(GameObject go) {
			shader.SetUniform("texture_diffuse1", go.texture);
			shader.SetTexture("texture_normal1", go.normalMap ?? normalMap);
			shader.SetTexture("texture_roughness1", go.roughnessMap ?? defaultRoughnessMap);
			shader.SetTexture("texture_ao1", go.aoMap ?? defaultAOMap);

			shader.SetUniform("useLayering", 0);
			shader.SetUniform("useNormalMap", (go.normalMap != null) ? 1 : 0);
			shader.SetUniform("useRoughnessMap", (go.roughnessMap != null) ? 1 : 0);
			shader.SetUniform("useAOMap", (go.aoMap != null) ? 1 : 0);




		}


		static void GenerateInstanceData() {
            Dictionary<Model, List<GameObject>> modelsToInsta = new Dictionary<Model, List<GameObject>>();

            foreach (var goTop in scene.gameObjects)
            {
				foreach (var goChild in goTop.children)
				{
                    if (goChild.useInstancing) {
                        if (!modelsToInsta.ContainsKey(goChild.model))
                        {
                            modelsToInsta.Add(goChild.model, new List<GameObject>());
                        }
						modelsToInsta.TryGetValue(goChild.model, out List<GameObject> list);
						list.Add(goChild);

					}
				}
				if (goTop.useInstancing)
				{
					if (!modelsToInsta.ContainsKey(goTop.model))
					{
						modelsToInsta.Add(goTop.model, new List<GameObject>());
					}
					modelsToInsta.TryGetValue(goTop.model, out List<GameObject> list);
					list.Add(goTop);

				}
			}

            //Create Vertex Bindigns

            foreach (Model key in modelsToInsta.Keys)
            {
				modelsToInsta.TryGetValue(key, out List<GameObject> list);

				InstanceMatrix[] instances = new InstanceMatrix[list.Count];
                for (int i = 0; i < list.Count; i++) {
                    instances[i] = new InstanceMatrix(list[i].transform.getModelMatrix());
                }

				VertexBuffer instanceVertexBuffer = new VertexBuffer(graphics,
					typeof(InstanceMatrix),
					list.Count,
					BufferUsage.WriteOnly);

				instanceVertexBuffer.SetData(instances);

				

				ModelMeshPart part = key.Meshes[0].MeshParts[0];
				int vertexOffset = part.VertexOffset;

				VertexBuffer vertexBuffer = part.VertexBuffer;

                VertexBufferBindingGroup vertexBufferBinding = new VertexBufferBindingGroup();
                vertexBufferBinding.vertexBufferBindings[0] = new VertexBufferBinding(vertexBuffer);//, vertexOffset, 0); // slot 0 – dane modelu
				vertexBufferBinding.vertexBufferBindings[1] = new VertexBufferBinding(instanceVertexBuffer, 0, 1); // slot 1 – instancje
				vertexBufferBinding.InstanceCount = instances.Length;


				InstancesQueue.Add(list[0], vertexBufferBinding);
			}

            int a = 0;

		}

			static void DrawInstanceData()
		{

            shader.SetUniform("useInstancing", 1);

            foreach (GameObject representative in InstancesQueue.Keys) {

				ModelMeshPart part = representative.model.Meshes[0].MeshParts[0];

				IndexBuffer indexBuffer = part.IndexBuffer;

				// Potrzebne do rysowania
				int vertexOffset = part.VertexOffset;
				int startIndex = part.StartIndex;
				int primitiveCount = part.PrimitiveCount;

				InstancesQueue.TryGetValue(representative, out var instances);

                /*graphics.SetVertexBuffers(instances);
				graphics.Indices = indexBuffer;*/

                /*graphics.DrawInstancedPrimitives(
	                PrimitiveType.TriangleList,
	                baseVertex: part.VertexOffset,
	                startIndex: part.StartIndex,
	                primitiveCount: part.PrimitiveCount,
	                instanceCount: 1//instances[1].VertexBuffer.VertexCount
				);*/

                setMaterial(representative);


				foreach (EffectPass pass in shader.Effect.CurrentTechnique.Passes)
				{
					pass.Apply();

					// Now call draw function
					graphics.SetVertexBuffers(instances.vertexBufferBindings);
					graphics.Indices = indexBuffer;

					graphics.DrawInstancedPrimitives(
						PrimitiveType.TriangleList,
						baseVertex: part.VertexOffset,
						startIndex: part.StartIndex,
						primitiveCount: part.PrimitiveCount,
						instanceCount: instances.InstanceCount
					);
				}

			}

			shader.SetUniform("useInstancing", 0);



		}
	}

            
}