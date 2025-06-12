namespace SolidSilnique.Core.Animation
{
    public class AnimationClip
    {

        public AnimationCurve PositionCurve = new AnimationCurve();
        public AnimationCurve RotationCurve = new AnimationCurve();
        public AnimationCurve ScaleCurve = new AnimationCurve();

        // Clip length in seconds (max of all curves).
        public float Duration
        {
            get
            {
                float d = 0;
                if (PositionCurve.Duration > d) d = PositionCurve.Duration;
                if (RotationCurve.Duration > d) d = RotationCurve.Duration;
                if (ScaleCurve.Duration > d) d = ScaleCurve.Duration;
                return d;
            }
        }
    }
}
