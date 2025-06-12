using Microsoft.Xna.Framework;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SolidSilnique.Core
{
	public class Transform
	{
		//Locals (Globals in future maybe?)
		Vector3 _position	= new Vector3(0);
		Vector3 _rotation	= new Vector3(0);
		Vector3 _scale		= new Vector3(1);

		public Vector3 position
		{
			get { return _position; }
			set { _position = value; dirtyFlag = true; }
		}
		public Vector3 rotation
		{
			get { return _rotation; }
			set { _rotation = value; dirtyFlag = true; }
		}
		public Vector3 scale
		{
			get { return _scale; }
			set { _scale = value; dirtyFlag = true; }
		}

		public Vector3 globalPosition {
			get { return _position + ((gameObject.parent != null) ? gameObject.parent.transform.globalPosition : Vector3.Zero); }
		}

		public Vector3 globalRotation
		{
			get { return _rotation + ((gameObject.parent != null) ? gameObject.parent.transform.globalRotation : Vector3.Zero); }
		}

		public Vector3 Forward { 
			get { return modelMatrix.Forward; }
		}

		//Model Matrix
		[JsonIgnore]
		public Matrix modelMatrix = new Matrix();

		//Flags
		bool _dirtyFlag = true;

		[JsonIgnore]
		public bool dirtyFlag
		{
			get { return _dirtyFlag; }
			set {
				_dirtyFlag = value;
				foreach (var child in gameObject.children) { 
					child.transform.dirtyFlag = value;
				}
			}
		}

		//gameObject
		public GameObject gameObject = null;


		public Transform(GameObject gameObject)
		{
						
			this.gameObject = gameObject;
		}

		public Matrix getModelMatrix() {
			if (_dirtyFlag) {

				modelMatrix = Matrix.CreateScale(_scale) *
								Matrix.CreateRotationX(MathHelper.ToRadians(_rotation.X)) *
								Matrix.CreateRotationY(MathHelper.ToRadians(_rotation.Y)) *
								Matrix.CreateRotationZ(MathHelper.ToRadians(_rotation.Z)) *
								Matrix.CreateTranslation(_position);
				if(gameObject.parent != null)
				{
					modelMatrix = modelMatrix * gameObject.parent.transform.getModelMatrix();
				}
				//modelMatrix =  * modelMatrix;

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

	public class Vector3Converter : JsonConverter<Vector3>
	{
		public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			// Reading a JSON object representing a Vector3
			using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
			{
				var root = doc.RootElement;

				// Extract X, Y, Z values from the JSON
				float x = root.GetProperty("X").GetSingle();
				float y = root.GetProperty("Y").GetSingle();
				float z = root.GetProperty("Z").GetSingle();

				return new Vector3(x, y, z);
			}
		}

		public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
		{
			// Writing the Vector3 as a JSON object with X, Y, Z components
			writer.WriteStartObject();
			writer.WriteNumber("X", value.X);
			writer.WriteNumber("Y", value.Y);
			writer.WriteNumber("Z", value.Z);
			writer.WriteEndObject();
		}
	}
}
