using SolidSilnique.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
	static class EngineManager
	{
		static Scene scene;
		static Stack<GameObject> renderQueue = [];

		static void Start()
		{

		}

		static void Update() 
		{ 
			
		}

		static void Draw()
		{
			while (renderQueue.Count > 0)
			{

				renderQueue.Pop();
			}
		}
	}
}
