using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace SolidSilnique.Core.Components
{
	public class TPPCameraComponent : Component
	{

		public float yaw, pitch = 0;
		
		public override void Start()
		{
			EngineManager.InputManager.MouseMoved += OnMouseMoved;
		}

		

		public override void Update()
		{
			gameObject.transform.rotation = new Vector3(pitch, -yaw, 0);// + Right * Pitch;
			//gameObject.children[0].GetComponent<CameraComponent>().camera.UpdateCameraVectors();
		}

		public void OnMouseMoved(float x, float y)
		{
			yaw += x * Time.deltaTime * 10;
			pitch -= y * Time.deltaTime * 10;

			//Yaw += xOffset;
			//Pitch += yOffset;


			if (pitch > 89.0f)
			{
				pitch = 89.0f;
			}

			if (pitch < -89.0f)
			{
				pitch = -89.0f;
			}
		}

		
	}
}
