using Microsoft.Xna.Framework;

namespace SolidSilnique
{
    public abstract class Light
    {
        private Vector3 ambientColor;
        private Vector3 diffuseColor;
        private Vector3 reflectiveColor;
        
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

        public Vector3 ReflectiveColor
        {
            get { return reflectiveColor; }
            set { reflectiveColor = value; }
        }
    }
}