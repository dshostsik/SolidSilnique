using Microsoft.Xna.Framework;

namespace SolidSilnique
{
    public class PointLight : Light
    {
        private float linear;
        private float quadratic;
        private float constant;

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

        public float Constant
        {
            get => constant;
            set => constant = value;
        }

        public PointLight(float linear, float quadratic, float constant)
        {
            this.linear = linear;
            this.quadratic = quadratic;
            this.constant = constant;
            AmbientColor = new Vector3(.2f, .2f, .2f);
            DiffuseColor = new Vector3(.8f, .8f, .8f);
            SpecularColor = new Vector3(.8f, .8f, .8f);
        }
    }
}