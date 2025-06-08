using Microsoft.Xna.Framework;
using SolidSilnique.Core.Interfaces;

namespace SolidSilnique.Core.Components
{
    /// <summary>
    /// Class used for enabling and defining interactions between two <see cref="GameObject"/>s.
    /// <p>The presence of this object already defines that the object can be interacted with.</p>
    /// </summary>
    public class InteractionComponent : Component, IInteractive
    {
        /// <summary>
        /// Flag defining if the object is currently interactive.
        /// </summary>
        private bool _currentlyInteractive;

        /// <summary>
        /// Flag defining if the object is currently interactive.
        /// </summary>
        public bool CurrentlyInteractive => _currentlyInteractive;

        private bool _wasInteracted = false;
        
        /// <summary>
        /// Target <see cref="GameObject"/> that will interact with <see cref="Self"/>. Primarily player object
        /// </summary>
        private GameObject _target;

        /// <summary>
        /// Target <see cref="GameObject"/> that will interact with <see cref="Self"/>. Primarily player object
        /// </summary>
        public GameObject Target
        {
            get => _target;
            set
            {
                if (null != value && gameObject.Equals(value))
                    throw new System.ArgumentException(
                        "Target cannot be set to itself.\nSet another object instead!");
                _target = value;
            }
        }

        /// <summary>
        /// Distance at which <see cref="Self"/> is interactive.
        /// </summary>
        private float _interactionDistance;

        /// <summary>
        /// Distance at which <see cref="Self"/> is interactive.
        /// </summary>
        public float InteractionDistance
        {
            get => _interactionDistance;
            set
            {
                if (value < 0)
                    throw new System.ArgumentException(
                        "Invalid argument!\nInteraction distance must be greater or equal to 0.0f\nOtherwise you'd have caused a bug of infinite following interrupted by collisions");
                var collider = gameObject.GetComponent<SphereColliderComponent>() ??
                               throw new System.ArgumentException("Game object must contain SphereColliderComponent");
                if (value < collider.boundingSphere.Radius)
                {
                    throw new System.ArgumentException(
                        "Invalid argument!\nInteraction distance must be greater or equal to radius, try again!\nOtherwise you'd have caused a bug of infinite following interrupted by collisions");
                }

                _interactionDistance = value;
            }
        }

        /// <summary>
        /// Multiplier to count <see cref="InteractionDistance"/>
        /// </summary>
        private float _distanceMultiplier;

        public float DistanceMultiplier
        {
            get => _distanceMultiplier;
            set
            {
                if (1.0f > value)
                    throw new System.ArgumentException(
                        "Invalid argument!\nDistance multiplier must be greater or equal to 1.0f\nOtherwise you'd have caused a bug of entering into collider to be able to interact with.");
                _distanceMultiplier = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="self">Reference to an interactive <see cref="GameObject"/></param>
        /// <param name="target">Reference to a <see cref="GameObject"/> that can interact with <see cref="Self"/></param>
        /// <param name="distanceMultiplier">Multiplier used to compute <see cref="InteractionDistance"/></param>
        /// <param name="multiplyInteractive">Flag that enables multiple interaction</param>
        /// <param name="shouldFollow">Flag that enables following for <see cref="Self"/> towards <see cref="Target"/></param>
        public InteractionComponent(GameObject self, GameObject target, float distanceMultiplier = 1.5f,
            bool multiplyInteractive = true, bool shouldFollow = false)
        {
            gameObject = self;
            _target = target;
            _distanceMultiplier = distanceMultiplier;
            var collider = gameObject.GetComponent<SphereColliderComponent>() ??
                           throw new System.ArgumentException("Game object must contain SphereColliderComponent");
            _interactionDistance = collider.boundingSphere.Radius * _distanceMultiplier;
        }

        /// <summary>
        /// Executed once at the start of the program. 
        /// </summary>
        public override void Start()
        {
        }


        /// <summary>
        /// Executed every frame. DO NOT ALLOCATE MEMORY HERE!
        /// </summary>
        /// <exception cref="System.NullReferenceException">if <see cref="Component.gameObject"/> is <c>null</c></exception>
        public override void Update()
        {
            if ((_interactionDistance * _interactionDistance) > SquaredDistanceBetweenTargetAndSelf() &&
                !_currentlyInteractive)
            {
                Pick();
            }
            else if (_currentlyInteractive && !_wasInteracted)
            {
                Release();
            }
        }

        public void Pick()
        {
            _currentlyInteractive = true;
        }

        public void Release()
        {
            _currentlyInteractive = false;
            _wasInteracted = true;
        }

        public float SquaredDistanceBetweenTargetAndSelf()
        {
            return Vector3.DistanceSquared(_target.transform.position, gameObject.transform.position);
        }
    }
}