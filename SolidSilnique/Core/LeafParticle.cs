using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
    internal class LeafParticle
    {
        /*
        struct ParticleVertex
        {
            public Vector3 InitialPos;
            public Vector3 Velocity;
            public float SpawnTime;
            // 4-component for 16-byte alignment if needed
        }

        private VertexBuffer _vb;
        private ParticleVertex[] _particles;
        private int _count;
        private Shader _shader;
        private Texture2D _leafTex;

        private float _lifeTime = 5f;
        private Vector3 _gravity = new Vector3(0, -9.8f, 0);

        public LeafParticle(int maxParticles)
        {
            _count = maxParticles;
            _particles = new ParticleVertex[_count];
        }

        public void LoadContent(GraphicsDevice gd, ContentManager content)
        {
            
            Random rnd = new Random();
            for (int i = 0; i < _count; i++)
            {
                float angle = MathHelper.TwoPi * (float)rnd.NextDouble();
                float radius = 1 + (float)rnd.NextDouble() * 2f;
                Vector3 pos = new Vector3(
                    250 + radius * (float)Math.Cos(angle),
                    15 + (float)rnd.NextDouble() * 2f,
                    220 + radius * (float)Math.Sin(angle)
                );
                Vector3 vel = new Vector3(
                    (float)(rnd.NextDouble() - 0.5) * 0.5f,
                    -(2 + (float)rnd.NextDouble() * 1f),
                    (float)(rnd.NextDouble() - 0.5) * 0.5f
                );
                _particles[i] = new ParticleVertex
                {
                    InitialPos = pos,
                    Velocity = vel,
                    SpawnTime = 0f
                };
            }

            // Upload to a GPU-only buffer
            var decl = new VertexDeclaration(
    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
    new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0)
    );
            _vb = new VertexBuffer(
            gd,
            decl,
            _count,
            BufferUsage.None
                );
            _vb.SetData(_particles);

            //Load effect and tex
            _shader = new Shader("Shaders/LeafParticle", gd, /*game=*//*null, "DrawLeafParticles");
            _leafTex = content.Load<Texture2D>("Textures/leaf_diffuse");
        }

        public void Draw(GraphicsDevice gd, Matrix view, Matrix proj, float totalTime)
        {
            // Set up states
            gd.RasterizerState = RasterizerState.CullNone;
            gd.DepthStencilState = DepthStencilState.Default;

            // Set buffers + effect parameters
            gd.SetVertexBuffer(_vb);

            _shader.SetUniform("currentTime", totalTime);
            _shader.SetUniform("lifeTime", _lifeTime);
            _shader.SetUniform("gravity", _gravity);
            _shader.SetUniform("View", view);
            _shader.SetUniform("Projection", proj);
            _shader.SetTexture("LeafTexture", _leafTex);

            foreach (var pass in _shader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawPrimitives(PrimitiveType.PointList, 0, _count);
            }
        }
        */
    }
}
