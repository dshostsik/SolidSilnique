using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
	abstract class Component
	{
		//gameObject
		public GameObject gameObject;

		abstract public void Start();

		abstract public void Update();

	}
}
