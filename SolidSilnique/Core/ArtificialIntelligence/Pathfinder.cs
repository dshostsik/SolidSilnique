#nullable enable
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolidSilnique.ProcderuralFoliage;

namespace SolidSilnique.Core.ArtificialIntelligence
{
    /// <summary>
    /// Class made for finding a path between two objects.
    /// </summary>
    public class Pathfinder : Component
    {
        /// <summary>
        /// Reference to self - object that should find the path to target
        /// </summary>
        private readonly GameObject _self;

        /// <summary>
        /// Reference to self - object that should find the path to target
        /// </summary>
        public GameObject Self => _self;

        /// <summary>
        /// Generated map.
        /// </summary>
        private readonly ProceduralGrass _terrain;

        /// <summary>
        /// Collection of points that self should follow to reach the target
        /// </summary>
        private List<Vector3> _path;

        /// <summary>
        /// Reference to target - object that self should reach
        /// </summary>
        private GameObject? _target;

        /// <summary>
        /// Point the <see cref="Self"/> <see cref="GameObject"/> should reach
        /// </summary>
        private Vector3? _destination;

        /// <summary>
        /// Point the <see cref="Self"/> <see cref="GameObject"/> should reach
        /// </summary>
        public Vector3? Destination
        {
            get => _destination;
            set => _destination = value;
        }

        /// <summary>
        /// Counted distance between self and target
        /// </summary>
        private float _distance;
        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="self">reference to self - object that should find the path to target</param>
        /// <param name="terrain">reference to the generated map</param>
        public Pathfinder(GameObject self, ProceduralGrass terrain)
        {
            _self = self;
            _terrain = terrain;
            _path = new();
            _distance = 0;
        }

        /// <summary>
        /// Collection of points that self should follow to reach the target
        /// </summary>
        public List<Vector3> Path => _path;

        /// <summary>
        /// Reference to target - object that self should reach
        /// </summary>
        public GameObject? Target
        {
            get => _target;
            set
            {
                if (null != value && _self.Equals(value)) throw new System.ArgumentException("Target cannot be set to itself.\nSet another object instead!");
                _target = value;
            }
        }
        
        /// <summary>
        /// Counts distance between self and target
        /// </summary>
        private void ComputePath()
        {
            if (_target == null) return;

            _distance = Vector3.Distance(_self.transform.position, _target.transform.position);
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}