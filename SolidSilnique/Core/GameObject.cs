using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SolidSilnique.Core
{
	public class GameObject
    {

		//Identifiers
		public string name = "GameObject";
		public int tag = 0;
		public int layer = 0;

		//Scene Graph
		public GameObject parent = null;
		public Transform transform = null;
		public List<GameObject> children = [];

		//Rendering
		public Model model = null;
		public Texture2D texture = null;

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
            EngineManager.renderQueue.Enqueue(this);

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
            child.parent = this;
            children.Add(child);

        }

		public void RemoveChild(GameObject child)
		{
			children.Remove(child);

		}

        //COMPONENTS
        /// <summary>
        /// COMPONENTY DODAWAĆ TYLKO TĄ METODĄ
        /// </summary>
        /// <param name="component"></param>
        public void AddComponent(Component component)
        {
            component.gameObject = this;
            components.Add(component);
        }

		
		/// <summary>
		/// SZUKA COMPONENTU PO TYPIE
		/// </summary>
		/// <param name="component"></param>
		public T GetComponent<T>() where T : Component
		{
			foreach (var component in components)
			{
                if (component is T) { 
                    return (T)component;
                }
			}
            return null;

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
