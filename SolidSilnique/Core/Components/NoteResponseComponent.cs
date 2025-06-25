using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidSilnique.Core;

namespace SolidSilnique.Core.Components
{
    public enum NodDirection { Forward, Backward, Left, Right }

    public class NoteResponseComponent : Component
    {
        private const float DURATION = 0.3f;
        // max nod angle
        private const float MAX_ANGLE = 30f;

        private float _elapsed;
        private bool _isNodding;
        private Vector3 _baseRotation;
        private NodDirection _dir;

        public override void Start()
        {
            _baseRotation = gameObject.transform.rotation;
        }

        public void Trigger(NodDirection dir)
        {
            _dir = dir;
            _elapsed = 0f;
            _isNodding = true;
        }

        public override void Update()
        {
            if (!_isNodding)
                return;

            _elapsed += Time.deltaTime;
            float t = _elapsed / DURATION;

            if (t >= 1f)
            {
                gameObject.transform.rotation = _baseRotation;
                _isNodding = false;
                return;
            }
            float angle = (float)Math.Sin(Math.PI * t) * MAX_ANGLE;

            Vector3 euler = _baseRotation;
            switch (_dir)
            {
                case NodDirection.Forward:
                    euler.X += angle;     // pitch down
                    break;
                case NodDirection.Backward:
                    euler.X -= angle;     // pitch up
                    break;
                case NodDirection.Left:
                    euler.Z += angle;     // roll left
                    break;
                case NodDirection.Right:
                    euler.Z -= angle;     // roll right
                    break;
            }

            gameObject.transform.rotation = euler;
        }

    }
}
