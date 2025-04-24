using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SolidSilnique.Core
{
	class GameObject
    {
        //Identifiers
        string name = "GameObject";
        int tag = 0;
        int layer = 0;

        //Scene Graph
        GameObject parent = null;
        Transform transform = null;
        List<GameObject> children = [];

        //Rendering
        Model model = null;
        Texture2D texture = null;

		//Components
		List<Component> components = [];

		public GameObject(string name)
        {
            this.name = name;
            this.transform = new Transform(this);
        }


        /// <summary>
        /// Wykonywana na początku programu
        /// </summary>
        public void Start()
        {
			foreach (var child in children)
			{
                child.Start();
			}

			foreach (var component in components)
            {
				component.Start();
			}
        }

        /// <summary>
        /// Wykonywana w każdej klatce programu
        /// </summary>
        public void Update()
        {


			foreach (var child in children)
			{
				child.Update();
			}

			foreach (var component in components)
			{
				component.Update();
			}

		}

        /// <summary>
        /// Wykonywana w pętli rysowania
        /// </summary>
        public void Draw()
        {
			foreach (var child in children)
			{
				child.Draw();
			}

		}

        /// <summary>
        /// Służy do usuwania
        /// </summary>
        public void Destroy()
        {
            if (parent != null) {
                parent.RemoveChild(this);
            }
        }


        //SCENE GRAPH


        public void AddChild(GameObject child)
        {
            children.Add(child);

        }

		public void RemoveChild(GameObject child)
		{
			children.Remove(child);

		}


		////Searching
		//By Name



		static GameObject FindGameObjectByName(string name)
        {

            throw new NotImplementedException();
        }

        static GameObject FindGameObjectsByName(string name)
        {

            throw new NotImplementedException();
        }

        //By Tag

        static GameObject FindGameObjectByTag(int tag)
        {

            throw new NotImplementedException();
        }

        static GameObject FindGameObjectsByTag(int tag)
        {

            throw new NotImplementedException();
        }





    }
}
