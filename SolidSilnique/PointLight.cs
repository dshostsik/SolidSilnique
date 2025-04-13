using Microsoft.Xna.Framework;

namespace SolidSilnique
{
    public class PointLight : Light
    {
        private float linear;
        private float quadratic;

        public float Linear
        {
            get => linear;
            set => linear = value;
        }

        public float Quadratic
        {
            get => quadratic;
            set => quadratic = value;
        }

        public PointLight(float linear, float quadratic)
        {
            this.linear = linear;
            this.quadratic = quadratic;
            AmbientColor = new Vector3(.2f, .2f, .2f);
            DiffuseColor = new Vector3(.8f, .8f, .8f);
            ReflectiveColor = new Vector3(.8f, .8f, .8f);
        }
    }
}