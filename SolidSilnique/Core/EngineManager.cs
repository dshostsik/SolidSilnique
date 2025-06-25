using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using SolidSilnique.Core.Components;
using GUIRESOURCES;

namespace SolidSilnique.Core
{
    internal enum ShadowResolution
    {
        Low = 1024,
        Medium = 2048,
        High = 4096,
        Ultra = 8192*2
    }

    static class EngineManager
    {
        public static Scene scene = null;

        public static Queue<GameObject> renderQueue = [];
		public static Dictionary<GameObject, VertexBufferBindingGroup> InstancesQueue = []; //representative, InstanceBuffer
        public static Queue<Tuple<Texture2D,Vector2,Color>> renderQueueUI = [];

        public static bool celShadingEnabled = false;
        public static GUI currentGui;

        private static Queue<GameObject> shadowsQueue;
        
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
        public static GraphicsDevice graphics;
        public static Shader shader;
        public static LightsManagerComponent lightsManager;



		private static RenderTarget2D _staticShadowMapRenderTarget;
        private static int _iterationsCounter = 0;
        private static Matrix lightViewProjection;
        private static Matrix _lightProjection =  Matrix.CreateOrthographic(768 * 1.41f, 768*1.41f, -512, 512);

        private static RenderTarget2D _sceneRenderTarget;
        private static RenderTarget2D tempRenderTarget;
        public static SpriteBatch _postSpriteBatch;
        public static Effect _postProcessEffect;

        public static GraphicsDeviceManager GraphicsManager;
        public static Skybox Skybox;
        public static Input InputManager;
        public static bool mouseFree = false;

        public static LeafParticle LeafSystem1;
        public static LeafParticle LeafSystem2;

        public static SpriteBatch UiRenderer;
		public static void Start()
        {
			UiRenderer = new SpriteBatch(graphics);

			scene.Start();
            GenerateInstanceData();
        }

        public static void Update(GameTime gameTime)
        {
            Time.deltaTimeMs = gameTime.ElapsedGameTime.Milliseconds;
            Time.deltaTime = Time.deltaTimeMs / 1000.0f;
            scene.Update();
			EngineManager.scene.mainCamera.UpdateCameraVectors();
		}

        public static void InitializeInput(Game1 game)
        {
            InputManager = new Input(game);

            // Subscribe all the handlers here instead of in Game1
            InputManager.ActionPressed += OnActionPressed;
            InputManager.ActionHeld += OnActionHeld;
            InputManager.MouseMoved += OnMouseMoved;
            InputManager.MouseClicked += OnMouseClicked;
        }
        public static void ProcessInput(GameTime gameTime)
        {
            InputManager?.Process(gameTime);
        }

        private static RenderTarget2D BakeStaticShadows(Shader shadowShader, LightsManagerComponent manager)
        {
			//sh.SetUniform("useInstancingShadows", 0);
			RenderTarget2D output = new RenderTarget2D(graphics, _testSettings,
                _testSettings, false,
                SurfaceFormat.Single, DepthFormat.Depth24);
            shadowsQueue = new Queue<GameObject>(renderQueue);
            // Drawing shadows
            graphics.SetRenderTarget(output);
            graphics.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            Matrix lightView = Matrix.CreateLookAt(manager.DirectionalLightPosition - manager.DirectionalLight.Direction,
                manager.DirectionalLightPosition,
                Vector3.Up);

            lightViewProjection = lightView * _lightProjection;

            shadowShader.SwapTechnique("ShadeTheSceneRightNow");
            shadowShader.SetUniform("LightViewProj", lightViewProjection);
			//shader.SetUniform("Dimensions", new Vector2(output.Width, output.Height));
			graphics.RasterizerState = new RasterizerState
			{
				CullMode = CullMode.None,
				DepthBias = (1 / _testSettings) * 1024 * 1e+17f,
				SlopeScaleDepthBias = 4f
			};

			if (scene.environmentObject != null)
            {
				//graphics.RasterizerState = RasterizerState.CullNone;
	            scene.environmentObject.DrawAllBuffersToShader(shadowShader);
            }

            


			

			while (shadowsQueue.Count > 0)
            {
                GameObject go = shadowsQueue.Dequeue();
                if (go.model == null || !go.isStatic)
                {
                    continue;
                }

                for (int j = 0; j < go.model.Meshes.Count; j++)
                {
                    var shadowMesh = go.model.Meshes[j];
                    

                    shadowShader.SetUniform("World", go.transform.getModelMatrix());
                    
                    for (int k = 0; k < shadowMesh.MeshParts.Count; k++)
                    {
                        var part = shadowMesh.MeshParts[k];
                        part.Effect = shadowShader.Effect;
                    }
                    shadowMesh.Draw();
                }
            }

			DrawInstanceData(shadowShader, "Shadows");

            
			// using (var stream = new FileStream("shadowMap.png", FileMode.Create))
			// {
			//     output.SaveAsPng(stream, output.Width, output.Height);
			// }
			graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            graphics.SetRenderTarget(null);
            //-------------------------------------
            
            return output;
        }
        
        
        public static void Draw( Shader shadowShader, Matrix view, Matrix projection, Shader PostProcessShader )
        {
            if (_sceneRenderTarget == null)
            {
	            
                _sceneRenderTarget = new RenderTarget2D(
                    graphics,
                    1920, 1080,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24);
                
                tempRenderTarget = new RenderTarget2D(
	                graphics,
	                1920, 1080,
	                false,
	                SurfaceFormat.Color,
	                DepthFormat.Depth24);
            }
            
        
            
            graphics.SetRenderTarget(_sceneRenderTarget);
            graphics.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            

            if (Skybox != null)
                    {
               var skyView = view;
                skyView.Translation = Vector3.Zero;
                var prevDepth = graphics.DepthStencilState;
                graphics.DepthStencilState = DepthStencilState.None;
                graphics.RasterizerState = RasterizerState.CullNone;


                Skybox.Draw(GraphicsManager, view);

                graphics.DepthStencilState = prevDepth;
            }
            

            

            graphics.DepthStencilState = DepthStencilState.Default;
            graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            scene.Draw();

            if (_iterationsCounter < 1)
            {
                _iterationsCounter++;
                _staticShadowMapRenderTarget = BakeStaticShadows(shadowShader, lightsManager);
            }

            shader.SetUniform("LightViewProj", lightViewProjection);
            shader.SetTexture("shadowMap", _staticShadowMapRenderTarget);
            shader.SetUniform("shadowMapResolution", _testSettings);
            shader.Effect.CurrentTechnique = shader.Effect.Techniques["BasicColorDrawingWithLights"];
			// Normal rendering 
			while (renderQueue.Count > 0)
            {
                GameObject go = renderQueue.Dequeue();

                _frustum.Matrix = view * projection;
                var position = go.transform.position;
                var distance = Vector3.Distance(
                    EngineManager.scene.mainCamera.CameraPosition,
                    position);

                if (go.LODModels != null && go.LODModels.Count > 0)
                    go.model = go.GetLODModel(distance);

                if (go.model == null)
                    continue;

                try
                {
                    Matrix model = go.transform.getModelMatrix();
                    setMaterial(go);

                    shader.SetUniform("World", model);
                    Matrix modelTransInv = Matrix.Transpose(Matrix.Invert(model));
                    shader.SetUniform("WorldTransInv", modelTransInv);
                    
                    
                    for (int i = 0; i < go.model.Meshes.Count; i++)
                    {
                        var mesh = go.model.Meshes[i];
                        if (useCulling)
                        {
                            var sphere = mesh.BoundingSphere.Transform(model);
                            bool visible = _frustum.Intersects(sphere);

                            if (useWireframe)
                            {
                                var prevRaster = graphics.RasterizerState;
                                wireframeEffect.View = view;
                                wireframeEffect.Projection = projection;
                                wireframeEffect.World = Matrix.Identity;
                                graphics.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };

                                var box = BoundingBox.CreateFromSphere(sphere);
                                var corners = box.GetCorners();
                                var lines = new VertexPositionColor[24];
                                Color wireColor = visible ? Color.Green : Color.Red;
                                int[,] edges = {
                            {0,1},{1,2},{2,3},{3,0},
                            {4,5},{5,6},{6,7},{7,4},
                            {0,4},{1,5},{2,6},{3,7}
                        };
                                int idx = 0;
                                for (int e = 0; e < edges.GetLength(0); e++)
                                {
                                    lines[idx++] = new VertexPositionColor(
                                        corners[edges[e, 0]],
                                        wireColor);
                                    lines[idx++] = new VertexPositionColor(
                                        corners[edges[e, 1]],
                                        wireColor);
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
                                graphics.RasterizerState = prevRaster;
                            }

                            if (!visible)
                                continue;
                        }

                        foreach (var part in mesh.MeshParts)
                        {
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
                                
                                mesh.Draw();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (scene.environmentObject != null)
                scene.environmentObject.Draw(_frustum);
            DrawInstanceData(shader);
            graphics.DepthStencilState = DepthStencilState.DepthRead;

            // Compute the current song/time or frame time; if you have a static Time.totalGameTime:
            float t = Time.deltaTime;

            // Draw each leaf system
            LeafSystem1?.Draw(graphics, view, projection, t);
            LeafSystem2?.Draw(graphics, view, projection, t);

            var vp = graphics.Viewport;
            
            
            
            _postProcessEffect.Parameters["PrevRenderedSampler+PrevRendered"].SetValue(_sceneRenderTarget);
            
            PostProcessShader.SwapTechnique("PostProcess");
            graphics.SetRenderTarget(tempRenderTarget);
			
            _postSpriteBatch.Begin(
	            SpriteSortMode.Immediate,
	            BlendState.Opaque,
	            SamplerState.LinearClamp,
	            DepthStencilState.None,
	            RasterizerState.CullNone,
	            PostProcessShader.Effect
            );
            
            

            _postSpriteBatch.Draw(
	            _sceneRenderTarget,
	            new Rectangle(0, 0, vp.Width, vp.Height),
	            Color.White
            );
			
            _postSpriteBatch.End();
            PostProcessShader.SwapTechnique("Bloom");
            _postProcessEffect.Parameters["PrevRenderedSampler+PrevRendered"].SetValue(tempRenderTarget);
            _postProcessEffect.Parameters["PrevRenderedSamplerColor+PrevRenderedColor"].SetValue(_sceneRenderTarget);
            graphics.SetRenderTarget(null);
            
            _postSpriteBatch.Begin(
	            SpriteSortMode.Immediate,
	            BlendState.Opaque,
	            SamplerState.LinearClamp,
	            DepthStencilState.None,
	            RasterizerState.CullNone,
	            PostProcessShader.Effect
            );
            
            

            _postSpriteBatch.Draw(
	            tempRenderTarget,
	            new Rectangle(0, 0, vp.Width, vp.Height),
	            Color.White
            );
			
            _postSpriteBatch.End();
            
            
            UiRenderer.Begin();
            while (renderQueueUI.Count > 0)
            {
	            var element = renderQueueUI.Dequeue();
	            UiRenderer.Draw(element.Item1, element.Item2, element.Item3);
            }
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
			shader.SetUniform("albedo", go.albedo.ToVector4());

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

			static void DrawInstanceData(Shader sh, string affix = "")
		{

            sh.SetUniform("useInstancing"+affix, 1);

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


				foreach (EffectPass pass in sh.Effect.CurrentTechnique.Passes)
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

			sh.SetUniform("useInstancing"+ affix, 0);



		}
        private static void OnActionPressed(string action)
        {
            var cam = scene.mainCamera;
            switch (action)
            {

                case "Up": cam.move(Camera.directions.UP, Time.deltaTime); break;
                case "Shoot": cam.cameraComponent.Shoot(); break;
                case "ToggleCulling":
                    useCulling = !useCulling;
                    break;
                case "ToggleWireframe":
                    useWireframe = !useWireframe;
                    break;
                case "ToggleCelShadingOn":
                    celShadingEnabled = true;
                    break;
                case "ToggleCelShadingOff":
                    celShadingEnabled = false;
                    break;
                case "SwitchCamera":
                    if (scene.TPCamera != null)
                    {
                        var tmp = scene.mainCamera;
                        InputManager.gMode = !InputManager.gMode;
                        scene.mainCamera = scene.TPCamera;
                        scene.TPCamera = tmp;
                    }
                    break;
                case "ToggleMouseFree":
                    mouseFree = !mouseFree;
                    break;
            }
        }

        private static void OnActionHeld(string action)
        {
            var cam = scene.mainCamera;
            float dt = Time.deltaTime;
            if(InputManager.gMode == false)
            {
                switch (action)
                {
                    case "Forward": cam.move(Camera.directions.FORWARD, dt); break;
                    case "Backward": cam.move(Camera.directions.BACKWARD, dt); break;
                    case "Left": cam.move(Camera.directions.LEFT, dt); break;
                    case "Right": cam.move(Camera.directions.RIGHT, dt); break;
                    case "Up": cam.move(Camera.directions.UP, dt); break;
                }
            }
            
        }

        private static void OnMouseMoved(float dx, float dy)
        {
            if (!mouseFree)
                scene.mainCamera.mouseMovement(dx, dy, Time.deltaTimeMs);
        }

        private static void OnMouseClicked(MouseButton button)
        {
            // optional: handle clicks if you need to
        }
    }

            
}