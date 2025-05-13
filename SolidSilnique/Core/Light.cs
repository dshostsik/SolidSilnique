using Microsoft.Xna.Framework;

namespace SolidSilnique.Core
{
    public abstract class Light  : Component
    {
        private Vector4 _ambientColor;
        private Vector4 _diffuseColor;
        private Vector4 _specularColor;

        private bool _enabled;

        protected Light()
        {
            _enabled = true;
        }

        public Vector4 AmbientColor
        {
            get => _ambientColor;
            set => _ambientColor = value;
        }

        public Vector4 DiffuseColor
        {
            get => _diffuseColor; 
            set => _diffuseColor = value; 
        }

        public Vector4 SpecularColor
        {
            get => _specularColor;
            set => _specularColor = value;
        }

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }
        
        /// <summary>
        /// Send all light's data to shader.
        /// </summary>
        /// <param name="shader">Shader object that accepts uniforms</param>
        /// <exception cref="Diagnostics.UniformNotFoundException">if uniform was not found in shader</exception>
        public abstract void SendToShader(Shader shader);
    }
}