using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SolidSilnique.Core.Animation;

namespace SolidSilnique.Core.Components
{
    public class AnimatorComponent : Component
    {
        private AnimationClip _clip;
        private float _time;
        private bool _loop;
        public bool IsPlaying { get; private set; }


        // Create with a clip; call Play() to start.
        public AnimatorComponent(AnimationClip clip, bool loop = true)
        {
            _clip = clip;
            _loop = loop;
        }

        // Starts or restarts the animation.
        public void Play()
        {
            _time = 0f;
            IsPlaying = true;
        }

        // Stops the animation (freezes at current pose).
        public void Stop() => IsPlaying = false;

        public override void Update()
        {
            if (!IsPlaying || _clip == null || _clip.Duration <= 0f)
                return;

            // advance time
            _time += Time.deltaTime;
            if (_time > _clip.Duration)
            {
                if (_loop)
                    _time %= _clip.Duration;
                else
                {
                    _time = _clip.Duration;
                    IsPlaying = false;
                }
            }

            // sample curves
            var pos = _clip.PositionCurve.Evaluate(_time);
            var rot = _clip.RotationCurve.Evaluate(_time);
            var scale = _clip.ScaleCurve.Evaluate(_time);

            // apply to transform
            gameObject.transform.position = pos;
            gameObject.transform.rotation = rot;    // assuming your Transform exposes 
            gameObject.transform.scale = scale;  // public fields for simplicity
        }

        public override void Start()
        {
        }
    }
}
