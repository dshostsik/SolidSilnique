using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SolidSilnique.Core.Components
{
	public class CameraComponent : Component
	{
		public Camera camera;

		public CameraComponent() {
			camera = new Camera(this);

		}
		public override void Start()
		{
			
		}

		public override void Update()
		{
			//gameObject.transform.position += new Vector3(0, -5, 0) * Time.deltaTime;
			
		}

		public void SetMain() {
			EngineManager.scene.mainCamera = camera;
		
		}

		public void Shoot()
		{
			GameObject toChange = Physics.PhysicsManager.Raycast(gameObject.transform.globalPosition, camera.Front, 10);
			if(toChange != null)
			{
				toChange.texture = EngineManager.scene.loadedTextures["deimos"];
			}


			


		}
	}
}
