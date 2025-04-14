using Microsoft.Xna.Framework;

namespace SolidSilnique
{
    public class DirectionalLight : Light
    {
        private Vector3 _direction;

        public DirectionalLight(Vector3 direction)
        {
            _direction = direction;
            AmbientColor = new Vector3(.2f, .2f, .2f);
            DiffuseColor = new Vector3(.8f, .8f, .8f);
            SpecularColor = new Vector3(.8f, .8f, .8f);
        }

        public Vector3 Direction
        {
            get => _direction;
            set => _direction = value;
        }
    }
}