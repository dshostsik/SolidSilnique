using Microsoft.Xna.Framework;

namespace SolidSilnique.Core
{
    public abstract class Light : GameObject
    {
        private Vector4 ambientColor;
        private Vector4 diffuseColor;
        private Vector4 specularColor;

        private bool enabled;

        protected Light(string name) : base(name)
        {
            enabled = true;
        }

        public Vector4 AmbientColor
        {
            get { return ambientColor; }
            set { ambientColor = value; }
        }

        public Vector4 DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        public Vector4 SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        
        /// <summary>
        /// Send all light's data to shader.
        /// </summary>
        /// <param name="shader">Shader object that accepts uniforms</param>
        /// <throws cref="UniformNotFoundException">if uniform was not found in shader</throws>
        public abstract void SendToShader(Shader shader);
    }
}