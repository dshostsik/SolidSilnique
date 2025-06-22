using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SolidSilnique.Core.Animation
{
    public class AnimationCurve
    {
        private readonly List<Keyframe<Vector3>> _keys = new List<Keyframe<Vector3>>();

        //Adds a keyframe; keys must be added in ascending time order.
        public void AddKey(Keyframe<Vector3> key) => _keys.Add(key);

        //Returns the interpolated Vector3 at time t.
        public Vector3 Evaluate(float t)
        {
            if (_keys.Count == 0)
                return Vector3.Zero;
            if (t <= _keys[0].Time)
                return _keys[0].Value;
            if (t >= _keys[^1].Time)
                return _keys[^1].Value;

            // find the segment
            for (int i = 0; i < _keys.Count - 1; i++)
            {
                var a = _keys[i];
                var b = _keys[i + 1];
                if (t >= a.Time && t <= b.Time)
                {
                    float frac = (t - a.Time) / (b.Time - a.Time);
                    return Vector3.Lerp(a.Value, b.Value, frac);
                }
            }
            return _keys[^1].Value;
        }

        public float Duration => _keys.Count > 0 ? _keys[^1].Time : 0f;
    }
}

