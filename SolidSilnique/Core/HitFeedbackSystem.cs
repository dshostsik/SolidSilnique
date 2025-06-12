using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SolidSilnique.Core
{
    public class HitFeedbackSystem
    {
        class Feedback
        {
            public Texture2D Texture;
            public Vector2 Position;
            public float StartTime;
            public float Duration;
            public float Rotation;  
        }

        private readonly List<Feedback> _feedbacks = new();
        private readonly Random _rand = new Random();
        public float TotalDuration = 0.5f;
        public float ScaleInTime = 0.2f;
        public float MaxRotationRad = MathHelper.ToRadians(15);

        public void AddFeedback(Texture2D tex, Vector2 pos, float songTime)
        {
            var rot = (float)(_rand.NextDouble() * 2 - 1) * MaxRotationRad;
            _feedbacks.Add(new Feedback
            {
                Texture = tex,
                Position = pos,
                StartTime = songTime,
                Duration = TotalDuration,
                Rotation = rot
            });
        }

        public void Update(float songTime)
        {
            for (int i = _feedbacks.Count - 1; i >= 0; i--)
            {
                var fb = _feedbacks[i];
                if (songTime - fb.StartTime > fb.Duration)
                    _feedbacks.RemoveAt(i);
            }
        }

        public void Render(SpriteBatch batch, float songTime)
        {
            if (_feedbacks.Count == 0) return;

            batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend);

            foreach (var fb in _feedbacks)
            {
                float elapsed = songTime - fb.StartTime;

                float scale = MathHelper.Clamp(elapsed / ScaleInTime, 0f, 1f);


                var origin = new Vector2(
                    fb.Texture.Width * 0.5f,
                    fb.Texture.Height * 0.5f);

                batch.Draw(
                    fb.Texture,
                    fb.Position,
                    null,               // sourceRect
                    Color.White,        // tint
                    fb.Rotation,        // random rotation
                    origin,
                    scale,              // uniform scale
                    SpriteEffects.None,
                    0f);
            }

            batch.End();
        }
    }
}
