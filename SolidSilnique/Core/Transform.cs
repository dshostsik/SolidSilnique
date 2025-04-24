using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
	class Transform
	{
		//Locals (Globals in future maybe?)
		Vector3 _position = new Vector3(0);
		Vector3 _rotation = new Vector3(0);
		Vector3 _scale = new Vector3(1);

		public Vector3 position
		{
			get { return _position; }
			set { _position = value; _dirtyFlag = true; }
		}
		public Vector3 rotation
		{
			get { return _rotation; }
			set { _rotation = value; _dirtyFlag = true; }
		}
		public Vector3 scale
		{
			get { return _scale; }
			set { _scale = value; _dirtyFlag = true; }
		}

		//Model Matrix
		public Matrix modelMatrix = new Matrix();

		//Flags
		bool _dirtyFlag = true;

		//gameObject
		public GameObject gameObject = null;


		public Transform(GameObject gameObject)
		{
						
			this.gameObject = gameObject;
		}

		public Matrix getModelMatrix() {
			if (_dirtyFlag) {

				modelMatrix =	Matrix.CreateScale(_scale) *
								Matrix.CreateRotationX(MathHelper.ToRadians(_rotation.X)) *
								Matrix.CreateRotationY(MathHelper.ToRadians(_rotation.Y)) *
								Matrix.CreateRotationZ(MathHelper.ToRadians(_rotation.Z)) *
								Matrix.CreateTranslation(_position);
				if(gameObject.parent != null)
				{
					modelMatrix = gameObject.parent.transform.getModelMatrix() * modelMatrix;
				}
				

				_dirtyFlag = false;
			}
			return modelMatrix;

		}

		//EDIT METHODS

		//public void Translate() { }
		//public void RotateX();
		//public void RotateY();
		//public void RotateZ();
		//public void Rotate();

	}
}
