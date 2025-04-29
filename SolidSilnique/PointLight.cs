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
            AmbientColor = new Vector4(.2f, .2f, .2f, .0f);
            DiffuseColor = new Vector4(.8f, .8f, .8f, .0f);
            SpecularColor = new Vector4(.8f, .8f, .8f, .0f);
        }

        public override void SendToShader(Shader shader)
        {
            try
            {
                shader.SetUniform("pointlight1Enabled", Enabled);
                // TODO: Integrate light objects inheritance from GameObject class
                // shader.SetUniform("pointlight1_position", pointlight_position);
                shader.SetUniform("pointlight1_ambientColor", AmbientColor);
                shader.SetUniform("pointlight1_diffuseColor", DiffuseColor);
                shader.SetUniform("pointlight1_specularColor", SpecularColor);
                shader.SetUniform("pointlight1_linearAttenuation", Linear);
                shader.SetUniform("pointlight1_quadraticAttenuation", Quadratic);
                shader.SetUniform("pointlight1_constant", Constant);
            }
            catch (UniformNotFoundException e)
            {
                throw new UniformNotFoundException(e.Message, " error source: PointLight.cs");
            }
        }
    }
}