using System;
using Microsoft.Xna.Framework;
using Vector3 = System.Numerics.Vector3;

namespace SolidSilnique
{
    /// <summary>
    /// Container of transform matrices of an object allowing to operate on them
    /// </summary>
    public class Transform
    {
        private Matrix _position;
        private Matrix _rotation;
        private Matrix _scale;

        public Matrix Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Matrix Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Matrix Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public Matrix GetWorldMatrix()
        {
            return _position * _rotation * _scale;
        }

        // TODO: Implement those functions
        /// <summary>
        /// Function to move transform on a defined distance
        /// </summary>
        /// <param name="velocity">Speed of movement or the distance object passes every frame</param>
        public void Move(Vector3 velocity)
        {
            //_position = Matrix.CreateTranslation(velocity, out _position);
            throw new NotImplementedException("This function is not implemented");
        }
        
        /// <summary>
        /// Function to move transform to the exact point in the world
        /// </summary>
        /// <param name="position">Exact postion</param>
        public void SetPosition(Vector3 position)
        {
            //_position = Matrix.CreateTranslation(position, Matrix.Identity);
            throw new NotImplementedException("This function is not implemented");
        }

        /// <summary>
        /// Rotate transform around X axis
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        public void RotateX(float angle)
        {
            _rotation *= Matrix.CreateRotationX(MathHelper.ToRadians(angle));
        }
        
        /// <summary>
        /// Rotate transform around Y axis
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        public void RotateY(float angle)
        {
            _rotation *= Matrix.CreateRotationY(MathHelper.ToRadians(angle));
        }
        
        /// <summary>
        /// Rotate transform around Z axis
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        public void RotateZ(float angle)
        {
            _rotation *= Matrix.CreateRotationZ(MathHelper.ToRadians(angle));
        }

        /// <summary>
        /// Rescale the transform into a wanted size
        /// </summary>
        /// <param name="scale">Coefficient of scaling of the object</param>
        public void Rescale(Vector3 scale)
        {
            _scale = Matrix.CreateScale(scale);
        }

        /// <summary>
        /// Rescale the object by X axis only
        /// </summary>
        /// <param name="factor">Coefficient of scaling of the object</param>
        public void RescaleX(float factor)
        {
            _scale = Matrix.CreateScale(new Vector3(factor, .0f, .0f));
        }
        
        /// <summary>
        /// Rescale the object by Y axis only
        /// </summary>
        /// <param name="factor">Coefficient of scaling of the object</param>
        public void RescaleY(float factor)
        {
            _scale = Matrix.CreateScale(new Vector3(.0f, factor, .0f));
        }
        
        /// <summary>
        /// Rescale the object by Z axis only
        /// </summary>
        /// <param name="factor">Coefficient of scaling of the object</param>
        public void RescaleZ(float factor)
        {
            _scale = Matrix.CreateScale(new Vector3(.0f, .0f, factor));
        }
    }
}