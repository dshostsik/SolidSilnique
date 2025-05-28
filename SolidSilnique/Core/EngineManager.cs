using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.GameContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidSilnique.Core.Components;
using SolidSilnique.Core.Diagnostics;

namespace SolidSilnique.Core
{
    internal enum ShadowResolution
    {
        Low = 512,
        Medium = 1024,
        High = 2048,
        Ultra = 4096
    }

    static class EngineManager
    {
        public static Scene scene = null;
        public static Queue<GameObject> renderQueue = [];
        public static bool celShadingEnabled = false;

        private static Queue<GameObject> shadowsQueue = new Queue<GameObject>();
        private static Vector3 offset = new Vector3(250,100f,250);
        
        //Debug flags
        public static bool useCulling = true;
        public static bool useWireframe = false;

        //Default maps
        public static Texture2D normalMap;
        public static Texture2D defaultRoughnessMap;
        public static Texture2D defaultAOMap;

        private static BoundingFrustum _frustum = new BoundingFrustum(Matrix.Identity);

        private static int _testSettings = (int)ShadowResolution.Ultra;

        public static BasicEffect wireframeEffect;

        internal static GraphicsDevice graphics;

        private static RenderTarget2D _shadowMapRenderTarget;

        private static Matrix _lightProjection =  Matrix.CreateOrthographic(512, 512, 0.01f, 10000f);

        public static void Start()
        {
            _shadowMapRenderTarget = new RenderTarget2D(graphics, _testSettings,
                _testSettings, false,
                SurfaceFormat.Single, DepthFormat.Depth24);
            scene.Start();
        }

        public static void Update(GameTime gameTime)
        {
            Time.deltaTimeMs = gameTime.ElapsedGameTime.Milliseconds;
            Time.deltaTime = Time.deltaTimeMs / 1000.0f;
            scene.Update();
        }

        public static void Draw(Shader shader, Shader shadowShader, Matrix view,
            Matrix projection, LightsManagerComponent manager)
        {
            scene.Draw();
            // TODO: fix this sh
            shadowsQueue = new Queue<GameObject>(renderQueue);
            // Drawing shadows
            graphics.SetRenderTarget(_shadowMapRenderTarget);
            graphics.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            Matrix lightView = Matrix.CreateLookAt(-manager.DirectionalLight.Direction*10 + offset,
                Vector3.Zero + offset,
                Vector3.Up);

            Matrix lightViewProjection = lightView * _lightProjection;

            shadowShader.SwapTechnique("ShadeTheSceneRightNow");
            graphics.RasterizerState = RasterizerState.CullClockwise;
            for (int i = 0; i < shadowsQueue.Count; i++)
            {
                GameObject go = shadowsQueue.Dequeue();
                if (go.model == null)
                {
                    continue;
                }

                for (int j = 0; j < go.model.Meshes.Count; j++)
                {
                    var shadowMesh = go.model.Meshes[j];
                    // if (useCulling)
                    // {
                    //     var sphere = mesh.BoundingSphere.Transform(go.transform.getModelMatrix());
                    //     
                    //     if (!_frustum.Intersects(sphere))
                    //         continue;
                    // }
                    
                    shadowShader.SetUniform("World", go.transform.getModelMatrix());
                    shadowShader.SetUniform("LightViewProj", lightViewProjection);
                    
                    for (int k = 0; k < shadowMesh.MeshParts.Count; k++)
                    {
                        var part = shadowMesh.MeshParts[k];
                        part.Effect = shadowShader.Effect;
                    }
                    shadowMesh.Draw();
                }
            }

            // using (var stream = new FileStream("shadowMap.png", FileMode.Create))
            // {
            //     _shadowMapRenderTarget.SaveAsPng(stream, _shadowMapRenderTarget.Width, _shadowMapRenderTarget.Height);
            // }
            graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            graphics.SetRenderTarget(null);
            //-------------------------------------
            
            // Normal rendering 
            while (renderQueue.Count > 0)
            {
                GameObject go = renderQueue.Dequeue();
                //FRUSTUM CULLING
                _frustum.Matrix = view * projection;

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

                    shader.SetUniform("useNormalMap", (go.normalMap != null) ? 1 : 0);
                    shader.SetUniform("useRoughnessMap", (go.roughnessMap != null) ? 1 : 0);
                    shader.SetUniform("useAOMap", (go.aoMap != null) ? 1 : 0);

                    shader.SetUniform("World", model);
                    Matrix modelTransInv = Matrix.Transpose(Matrix.Invert(go.transform.getModelMatrix()));
                    shader.SetUniform("WorldTransInv", modelTransInv);
                    shader.SetUniform("LightViewProj", lightViewProjection);
                    shader.SetTexture("shadowMap", _shadowMapRenderTarget);

                    for (int i = 0; i < go.model.Meshes.Count; i++)
                    {
                        ModelMesh mesh = go.model.Meshes[i];
                        if (useCulling)
                        {
                            var sphere = mesh.BoundingSphere.Transform(go.transform.getModelMatrix());
                            bool visible = _frustum.Intersects(sphere);

                            if (useWireframe)
                            {
                                var prevRasterizer = graphics.RasterizerState;


                                wireframeEffect.View = view;
                                wireframeEffect.Projection = projection;
                                wireframeEffect.World = Matrix.Identity;
                                // TODO: may be we can get rid of this somehow
                                graphics.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };


                                var box = BoundingBox.CreateFromSphere(sphere);
                                var corners = box.GetCorners();
                                // TODO: and this
                                var lines = new VertexPositionColor[24];
                                Color wireColor = visible ? Color.Green : Color.Red;
                                int idx = 0;
                                // TODO: and this
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
                        
                        for (int j = 0; j < mesh.MeshParts.Count; j++)
                        {
                            var part = mesh.MeshParts[j];
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
                    Console.WriteLine(e.Message, e.Source, e.StackTrace);
                    throw;
                }
                catch (UniformNotFoundException u)
                {
                    Console.WriteLine(u.Message);
                    throw;
                }
            }
            //-------------------------------------------
        }
    }
}