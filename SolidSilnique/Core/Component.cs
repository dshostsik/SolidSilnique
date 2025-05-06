using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core
{
	public abstract class Component
	{
		//gameObject
		public GameObject gameObject;

		public abstract void Start();

		public abstract void Update();

	}
}
