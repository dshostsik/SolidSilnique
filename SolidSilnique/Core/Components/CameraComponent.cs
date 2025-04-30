using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			
		}

		public void SetMain() {
			EngineManager.scene.mainCamera = camera;
		
		}
	}
}
