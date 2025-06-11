using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Animation
{
    public struct Keyframe<T>
    {
        public float Time;   // seconds
        public T Value;

        public Keyframe(float time, T value)
        {
            Time = time;
            Value = value;
        }
    }
}
