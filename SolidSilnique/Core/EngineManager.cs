using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.GameContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidSilnique.Core.Diagnostics;

namespace SolidSilnique.Core
{

    
    static class EngineManager
    {
        public static Scene scene = null;
        public static Queue<GameObject> renderQueue = [];
        public static Queue<Tuple<Texture2D,Vector2,Color>> renderQueueUI = [];
        public static bool celShadingEnabled = false;

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
                    shader.SetUniform("texture_diffuse1", go.texture);
                    shader.SetTexture("texture_normal1", go.normalMap ?? normalMap);
                    shader.SetTexture("texture_roughness1", go.roughnessMap ?? defaultRoughnessMap);
                    shader.SetTexture("texture_ao1", go.aoMap ?? defaultAOMap);

					shader.SetUniform("useLayering", 0);
					shader.SetUniform("useNormalMap", (go.normalMap != null) ? 1 : 0);
                    shader.SetUniform("useRoughnessMap", (go.roughnessMap != null) ? 1 : 0);
                    shader.SetUniform("useAOMap", (go.aoMap != null) ? 1 : 0);

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

            SpriteBatch UiRenderer = new SpriteBatch(graphics);
            UiRenderer.Begin();
            while(renderQueueUI.Count > 0)
            {
                Tuple<Texture2D,Vector2,Color> element = renderQueueUI.Dequeue();
                
                UiRenderer.Draw(element.Item1, element.Item2, element.Item3);
                
            }
            UiRenderer.End();
        }
    }
}