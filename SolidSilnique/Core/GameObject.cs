using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SolidSilnique.Core
{

	
	public class GameObject
    {

		//Identifiers
		public string name  { get; set; } = "GameObject";
        public int tag      { get; set; } = 0;
		public int layer    { get; set; } = 0;

		//Scene Graph
		[JsonIgnore]
		public GameObject parent = null;

		public Transform transform { get; set; } = null;
		
		public List<GameObject> children { get; set; } = [];

		//Rendering
		[JsonIgnore]
		public Model model = null;

		[JsonIgnore]
		public Texture2D texture = null;

        public string modelAssetName    { get; set; } = string.Empty;
        public string textureAssetName { get; set; } = string.Empty;

        public Texture2D normalMap = null;
        public Texture2D roughnessMap = null;
        public Texture2D aoMap = null;
        public Color albedo = Color.White;

        // Level of Detail (LOD)
        /// <summary>
        /// List of LOD models, sorted by increasing distance threshold.
        /// </summary>
        public List<Model> LODModels { get; private set; } = new List<Model>();
        
        /// <summary>
        /// Distance thresholds corresponding to each LODModel entry.
        /// </summary>
        public List<float> LODRanges { get; private set; } = new List<float>();


        public bool useInstancing = false;
        public bool isStatic = false;

        
        //Components
        [JsonInclude]
		List<Component> components { get; set; } = [];

        
		public GameObject(string name)
        {
            this.name = name;
            this.transform = new Transform(this);
        }


        public void AddLOD(Model lodModel, float minDistance)
        {
            LODModels.Add(lodModel);
            LODRanges.Add(minDistance);
        }

        /// <summary>
        /// Gets the appropriate Model for the given camera distance.
        /// </summary>
        public Model GetLODModel(float distance)
        {
            if (LODModels.Count == 0)
                return model;

            for (int i = LODRanges.Count-1; i >= 0; i--)
            {
                if (distance >= LODRanges[i])
                    return LODModels[i];
            }
            // Beyond all thresholds, return last LOD
            return LODModels[LODModels.Count - 1];
        }

        /// <summary>
        /// Executed at the start of the program.
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
        /// Executed in every frame.
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
        /// Executed in the render loop.
        /// </summary>
        public void Draw()
        {
            if (!useInstancing)
            {
				EngineManager.renderQueue.Enqueue(this);
			}
            

			foreach (var child in children)
			{
				child.Draw();
			}

		}

        /// <summary>
        /// Made to destroy game objects.
        /// </summary>
        public void Destroy()
        {

            if (parent != null)
            {
                parent.RemoveChild(this);
            }
            else { 
                //EngineManager.scene.RemoveChild(this);
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
        /// ADD COMPONENTS ONLY WITH THAT FUNCTION!
        /// </summary>
        /// <param name="component">component to de added</param>
        public void AddComponent(Component component)
        {
            component.gameObject = this;
            components.Add(component);
        }

		
		/// <summary>
		/// LOOKS FOR COMPONENT ACCORDING TO THE TYPE
		/// </summary>
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
