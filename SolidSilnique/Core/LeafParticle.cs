﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace SolidSilnique.Core
{
    internal class LeafParticle
    {
        struct ParticleVertex
        {
            public Vector3 InitialPos;
            public Vector3 Velocity;
            public float SpawnTime;
            public Vector2 Corner;

            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
            );
        }

        private VertexBuffer _vb;
        private ParticleVertex[] _vertices;
        private int _particleCount;
        private Shader _shader;
        private Texture2D _leafTex;
        public Game1 _game;

        private float _lifeTime = 20f;
        private Vector3 _gravity = new Vector3(0, -0.1f, 0);

        private float _accumulatedTime;
        //private readonly bool _infiniteLifetime;
        private float _timeAccumulator = 0f;

        public LeafParticle(int maxParticles, float lifeTime, Vector3 gravity)
        {
            _particleCount = maxParticles;
            _lifeTime = lifeTime;
            //_infiniteLifetime = (lifeTime <= 0f);
            _gravity = gravity;
            _vertices = new ParticleVertex[_particleCount * 6]; // 4 vertices per particle
            var rnd = new Random();
            _timeAccumulator = (float)rnd.NextDouble() * _lifeTime;

        }

        public void LoadContent(GraphicsDevice gd, ContentManager content, Texture2D particle)
        {
            Random rnd = new Random();

            // Quad corners in local space
            Vector2[] quadCorners = new[]
            {
                new Vector2(-1, -1), // 0
                new Vector2( 1, -1), // 1
                new Vector2(-1,  1), // 2
                new Vector2(-1,  1), // 3
                new Vector2( 1, -1), // 4
                new Vector2( 1,  1), // 5
            };

            int vi = 0;
            for (int i = 0; i < _particleCount; i++)
            {
                float angle = MathHelper.TwoPi * (float)rnd.NextDouble();
                float radius = 1 + (float)rnd.NextDouble() * 1000f;

                Vector3 pos = new Vector3(
                    320 + radius * (float)Math.Cos(angle),
                    10 + (float)rnd.NextDouble() * 100f,
                    350 + radius * (float)Math.Sin(angle)
                );

                Vector3 vel = new Vector3(
                    (float)(rnd.NextDouble() - 0.5) * 0.5f,
                    -(2 + (float)rnd.NextDouble() * 1f),
                    (float)(rnd.NextDouble() - 0.5) * 0.5f
                );
                Vector3 vel0 = new Vector3(0, 0, 0

                );

                float spawnTime = 0f;

                // Add 6 vertices per particle (2 triangles)
                for (int j = 0; j < 6; j++)
                {
                    _vertices[vi++] = new ParticleVertex
                    {
                        InitialPos = pos,
                        Velocity = vel0,
                        SpawnTime = spawnTime,
                        Corner = quadCorners[j]
                    };
                }
            }

            _vb = new VertexBuffer(
                gd,
                ParticleVertex.VertexDeclaration,
                _vertices.Length,
                BufferUsage.WriteOnly
            );
            _vb.SetData(_vertices);

            _shader = new Shader("Shaders/LeafParticle", gd, _game, "DrawLeafParticles");
            _leafTex = particle;
            //content.Load<Texture2D>("Textures/Dust")
        }


        
        public void Draw(GraphicsDevice gd, Matrix view, Matrix proj, float totalTime)
        {
            _accumulatedTime += totalTime;
            gd.RasterizerState = RasterizerState.CullNone;
            gd.DepthStencilState = DepthStencilState.DepthRead;
            gd.BlendState = BlendState.AlphaBlend;

            if (_accumulatedTime > _lifeTime)
                _accumulatedTime -= _lifeTime;


            gd.SetVertexBuffer(_vb);

            _shader.SetUniform("currentTime", _accumulatedTime);
            _shader.SetUniform("lifeTime", _lifeTime);
            _shader.SetUniform("gravity", _gravity);
            _shader.SetUniform("View", view);
            _shader.SetUniform("Projection", proj);
            _shader.SetTexture("LeafSampler", _leafTex);
            

            foreach (var pass in _shader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawPrimitives(PrimitiveType.TriangleList, 0, _particleCount * 2); // 2 triangles per quad
            }
        }
    }

}
