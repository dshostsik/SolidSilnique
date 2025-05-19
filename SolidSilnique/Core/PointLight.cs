using System;
using Microsoft.Xna.Framework;
using SolidSilnique.Core.Diagnostics;

namespace SolidSilnique.Core
{
    public class PointLight : Light
    {
        private float _linear;
        private float _quadratic;
        private float _constant;

        private static int _instances = 0;
        private readonly int _index;

        public static int PointLightInstances
        {
            get => _instances;
            // For inheritating class Spotlight
            protected set => _instances = value;
        }

        public int PointLightIndex => _index;

        public float Linear
        {
            get => _linear;
            set => _linear = value;
        }

        public float Quadratic
        {
            get => _quadratic;
            set => _quadratic = value;
        }

        public float Constant
        {
            get => _constant;
            set => _constant = value;
        }

        public PointLight(float linear, float quadratic, float constant)
        {
            _index = _instances;

            if (_instances < 10) _instances++;

            _linear = linear;
            _quadratic = quadratic;
            _constant = constant;
            AmbientColor = new Vector4(.1f, .1f, .1f, .0f);
            DiffuseColor = new Vector4(.8f, .8f, .8f, .0f);
            SpecularColor = new Vector4(1.0f, 1.0f, 1.0f, .0f);
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

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}