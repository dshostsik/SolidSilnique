using Microsoft.Xna.Framework;
using SolidSilnique.Core.ArtificialIntelligence;

namespace SolidSilnique.Core.Components
{
    /// <summary>
    /// Class used for enabling and defining interactions between two <see cref="GameObject"/>s.
    /// <p>The presence of this object already defines that the object can be interacted with.</p>
    /// </summary>
    public class InteractionComponent : Component
    {
        /// <summary>
        /// Flag defining if the object is following the player or another <see cref="GameObject"/>.
        /// </summary>
        private bool _isFollowing;

        /// <summary>
        /// Flag defining if the object is following the player or another <see cref="GameObject"/>. Is set once and forever
        /// </summary>
        public bool IsFollowing => _isFollowing;

        /// <summary>
        /// Flag defining if the object can be interacted with multiple times.
        /// </summary>
        private bool _isMultiplyInteractive;

        /// <summary>
        /// Flag defining if the object can be interacted with multiple times.
        /// </summary>
        public bool IsMultiplyInteractive
        {
            get => _isMultiplyInteractive;
            set => _isMultiplyInteractive = value;
        }

        /// <summary>
        /// Flag defining if the object is currently interactive.
        /// </summary>
        private bool _currentlyInteractive;
        
        /// <summary>
        /// Flag defining if the object is currently interactive.
        /// </summary>
        public bool CurrentlyInteractive => _currentlyInteractive;
        
        /// <summary>
        /// Flag defining if the object was interacted with. A bit pointless if <c>_multiplyInteractive == true</c>
        /// </summary>
        private bool _wasInteracted;

        /// <summary>
        /// Flag defining if the object was interacted with. A bit pointless if <c>_multiplyInteractive == true</c>
        /// </summary>
        public bool WasInteracted
        {
            get => _wasInteracted;
            set => _wasInteracted = value;
        }

        /// <summary>
        /// <see cref="GameObject"/> that can be interacted with.
        /// </summary>
        private GameObject _self;

        /// <summary>
        /// <see cref="GameObject"/> that can be interacted with.
        /// </summary>
        public GameObject Self => _self;

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
                if (null != value && _self.Equals(value))
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
                var collider = _self.GetComponent<SphereColliderComponent>() ?? throw new System.ArgumentException("Game object must contain SphereColliderComponent");
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
                if (value < 1.0f)
                    throw new System.ArgumentException(
                        "Invalid argument!\nDistance multiplier must be greater or equal to 1.0f\nOtherwise you'd have caused a bug of infinite following interrupted by collisions");
                if (value == _distanceMultiplier) return;
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
        /// <param name="isFollowing">Flag that enables following for <see cref="Self"/> towards <see cref="Target"/></param>
        /// <param name="wasInteracted">Flag which shows that the object was already in use. A bit pointless if <c>_multiplyInteractive == true</c></param>
        public InteractionComponent(GameObject self, GameObject target, float distanceMultiplier = 2.5f, bool multiplyInteractive = true, bool isFollowing = false,
            bool wasInteracted = false)
        {
            _self = self;
            _target = target;
            _distanceMultiplier = distanceMultiplier;
            var collider = _self.GetComponent<SphereColliderComponent>() ?? throw new System.ArgumentException("Game object must contain SphereColliderComponent");
            _interactionDistance = collider.boundingSphere.Radius * _distanceMultiplier;
            _isMultiplyInteractive = multiplyInteractive;
            _isFollowing = isFollowing;
            _wasInteracted = wasInteracted;
        }

        /// <summary>
        /// Executed once at the start of the program. 
        /// </summary>
        public override void Start()
        {
            _currentlyInteractive = false;
        }

        /// <summary>
        /// Executed every frame
        /// </summary>
        /// <exception cref="System.NullReferenceException">if <see cref="Component.gameObject"/> is <c>null</c></exception>
        public override void Update()
        {
            Vector3 distance = _self.transform.position - _target.transform.position;
            distance.Y = 0.0f;
            if (distance.LengthSquared() > (_interactionDistance * _interactionDistance)) return;
            if (_isFollowing)
            {
                var follower = new Follower(_self, DistanceMultiplier)
                {
                    Target = _target,
                    gameObject = gameObject
                };
                _self.AddComponent(follower);
                _isFollowing = false;
            }

            _currentlyInteractive = !_wasInteracted && _isMultiplyInteractive;
            if (!_isMultiplyInteractive) _wasInteracted = true;
        }
    }
}