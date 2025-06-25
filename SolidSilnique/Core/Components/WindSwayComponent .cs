using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Components
{
    public class WindSwayComponent : Component
    {
        public float AmplitudeDegrees = 10f;
        public float Frequency = 0.5f;
        public Vector3 SwayAxis = Vector3.Up;
        public bool UseWorldSpace = false;
        private float _timeAccumulator = 0f;

        // store the object’s original rotation so we can offset from it
        private Vector3 _baseEuler;

        public override void Start()
        {
            // capture the starting euler rotation once
            _baseEuler = gameObject.transform.rotation;
        }

        public override void Update()
        {
            // 1) advance our internal clock
            _timeAccumulator += Time.deltaTime;

            // 2) compute the current sway angle (radians):
            //    sine oscillates –1..1, scale by amplitude:
            float angleRad = MathHelper.ToRadians(AmplitudeDegrees)
                            * (float)System.Math.Sin(
                                2f * MathHelper.Pi * Frequency * _timeAccumulator
                              );

            // 3) build the sway quaternion:
            Quaternion swayQuat;
            if (UseWorldSpace)
            {
                swayQuat = Quaternion.CreateFromAxisAngle(SwayAxis, angleRad);
            }
            else
            {
                // compute the base object's orientation as a matrix
                var baseRotMatrix =
                    Matrix.CreateFromYawPitchRoll(
                        MathHelper.ToRadians(_baseEuler.Y),
                        MathHelper.ToRadians(_baseEuler.X),
                        MathHelper.ToRadians(_baseEuler.Z)
                    );
                // transform the axis into object space
                var localAxis = Vector3.TransformNormal(SwayAxis, baseRotMatrix);
                swayQuat = Quaternion.CreateFromAxisAngle(localAxis, angleRad);
            }

            // 4) compute the base rotation quaternion
            var baseQuat = Quaternion.CreateFromYawPitchRoll(
                MathHelper.ToRadians(_baseEuler.Y),
                MathHelper.ToRadians(_baseEuler.X),
                MathHelper.ToRadians(_baseEuler.Z)
            );

            // 5) combine: sway * base
            var finalQuat = Quaternion.Concatenate(swayQuat, baseQuat);

            // 6) convert back to Euler (degrees) and apply
            gameObject.transform.rotation = ToEuler(finalQuat);
        }

        // helper: convert quaternion to Euler (in degrees)
        private Vector3 ToEuler(Quaternion q)
        {
            // source: standard quaternion‐to‐Euler formulas
            Vector3 e;
            // pitch (X-axis rotation)
            float sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            float cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            e.X = MathHelper.ToDegrees((float)System.Math.Atan2(sinr_cosp, cosr_cosp));

            // yaw (Y-axis rotation)
            float sinp = 2 * (q.W * q.Y - q.Z * q.X);
            e.Y = MathHelper.ToDegrees(
                System.Math.Abs(sinp) >= 1
                ? System.Math.Sign(sinp) * MathHelper.PiOver2
                : (float)System.Math.Asin(sinp));

            // roll (Z-axis rotation)
            float siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            float cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            e.Z = MathHelper.ToDegrees((float)System.Math.Atan2(siny_cosp, cosy_cosp));

            return e;
        }
    }
}
