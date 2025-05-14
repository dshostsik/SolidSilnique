using System;
using Microsoft.Xna.Framework;
using SolidSilnique.Core.Diagnostics;

namespace SolidSilnique.Core
{
    public class Spotlight : PointLight
    {
        private Vector3 _direction;
        private float _innerCut;
        private float _outerCut;

        private static int _instances = 0;
        private readonly int _index;
        
        public Vector3 Direction
        {
            get => _direction; 
            set => _direction = value;
        }

        public float InnerCut
        {
            get => _innerCut; 
            set => _innerCut = value; 
        }

        public float OuterCut
        {
            get => _outerCut; 
            set => _outerCut = value; 
        }

        public Spotlight(float linear, float quadratic, float constant, Vector3 direction, float innerCut,
            float outerCut) :
            base(linear, quadratic, constant)
        {
            PointLight.PointLightInstances = PointLight.PointLightInstances - 1;
            _index = _instances;

            if (_instances < 10) _instances++;
            _direction = direction;
            _innerCut = innerCut;
            _outerCut = outerCut;
        }

        public static int SpotlightInstances => _instances;
        
        public int SpotlightIndex => _index;
        
        public override void SendToShader(Shader shader)
        {
            try
            {
                shader.SetUniform("spotlight1Enabled", Enabled);
                shader.SetUniform("spotlight1_direction", Direction);
                shader.SetUniform("spotlight1_innerCut", MathHelper.ToRadians(_innerCut));
                shader.SetUniform("spotlight1_outerCut", MathHelper.ToRadians(_outerCut));
                shader.SetUniform("spotlight1_linearAttenuation", Linear);
                shader.SetUniform("spotlight1_quadraticAttenuation", Quadratic);
                shader.SetUniform("spotlight1_constant", Constant);
                shader.SetUniform("spotlight1_ambientColor", AmbientColor);
                shader.SetUniform("spotlight1_diffuseColor", DiffuseColor);
                shader.SetUniform("spotlight1_specularColor", SpecularColor);
            }
            catch (UniformNotFoundException e)
            {
                throw new UniformNotFoundException(e.Message, " error source: Spotlight.cs");
            }
        }
        
        public override void Update()
        {
            throw new NotImplementedException();
        }
        
        public override void Start()
        {
            throw new NotImplementedException();
        }
    }
}