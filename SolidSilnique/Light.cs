using Microsoft.Xna.Framework;

namespace SolidSilnique
{
    public abstract class Light
    {
        private Vector3 ambientColor;
        private Vector3 diffuseColor;
        private Vector3 specularColor;

        private bool enabled;

        public Vector3 AmbientColor
        {
            get { return ambientColor; }
            set { ambientColor = value; }
        }

        public Vector3 DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        public Vector3 SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
    }
}