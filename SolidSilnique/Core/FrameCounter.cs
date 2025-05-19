using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SolidSilnique.Core
{
    /// <summary>
    /// Class counting current framerate
    /// <a href="https://stackoverflow.com/questions/20676185/xna-monogame-getting-the-frames-per-second">Link to StackOverflow</a>
    /// </summary>
    public class FrameCounter
    {
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float avgFPS { get; private set; }
        public float currentFPS { get; private set; }

        public const int MaximumSamples = 100;

        private Queue<float> samples = new();

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentFPS = 1.0f / deltaTime;
            
            samples.Enqueue(currentFPS);

            if (samples.Count > MaximumSamples)
            {
                samples.Dequeue();
                avgFPS = samples.Average(i => i);
            }
            else
            {
                avgFPS = currentFPS;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
        }
    }
}