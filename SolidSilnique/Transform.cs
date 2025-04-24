using Microsoft.Xna.Framework;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace SolidSilnique
{
    /// <summary>
    /// Container of transform matrices of an object allowing to operate on them
    /// </summary>
    public class TransformOld
    {
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;
        
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector3 Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(_scale) *
                   Matrix.CreateRotationX(_rotation.X) *
                   Matrix.CreateRotationY(_rotation.Y) *
                   Matrix.CreateRotationZ(_rotation.Z) *
                   Matrix.CreateTranslation(_position);
        }

        /// <summary>
        /// Function to move transform on a defined distance
        /// </summary>
        /// <param name="velocity">Speed of movement or the distance the object passes every frame</param>
        public void Move(Vector3 velocity)
        {
            _position += velocity;
        }

        /// <summary>
        /// Function to move transform to the exact point in the world
        /// </summary>
        /// <param name="position">Exact position</param>
        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        /// <summary>
        /// Rotate transform around X axis
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        public void RotateX(float angle)
        {
            _rotation.X += MathHelper.ToRadians(angle);
        }

        /// <summary>
        /// Rotate transform around Y axis
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        public void RotateY(float angle)
        {
            _rotation.Y += MathHelper.ToRadians(angle);
        }

        /// <summary>
        /// Rotate transform around Z axis
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        public void RotateZ(float angle)
        {
            _rotation.Z += MathHelper.ToRadians(angle);
        }

        /// <summary>
        /// Rescale the transform into a wanted size
        /// </summary>
        /// <param name="scale">Coefficient of scaling of the object</param>
        public void Rescale(Vector3 scale)
        {
            _scale = scale;
        }

        /// <summary>
        /// Rescale the object by X axis only
        /// </summary>
        /// <param name="factor">Coefficient of scaling of the object</param>
        public void RescaleX(float factor)
        {
            _scale = new Vector3(factor, _scale.Y, _scale.Z);
        }

        /// <summary>
        /// Rescale the object by Y axis only
        /// </summary>
        /// <param name="factor">Coefficient of scaling of the object</param>
        public void RescaleY(float factor)
        {
            _scale = new Vector3(_scale.X, factor, _scale.Z);
        }

        /// <summary>
        /// Rescale the object by Z axis only
        /// </summary>
        /// <param name="factor">Coefficient of scaling of the object</param>
        public void RescaleZ(float factor)
        {
            _scale = new Vector3(_scale.X, _scale.Y,  factor);
        }
    }
}