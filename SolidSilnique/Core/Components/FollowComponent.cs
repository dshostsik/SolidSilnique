using System;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using SolidSilnique.Core.ArtificialIntelligence;
using SolidSilnique.Core.Interfaces;

namespace SolidSilnique.Core.Components
{
    /// <summary>
    /// Component that defines behaviour "Interact and follow" for an object.
    /// </summary>
    public class FollowComponent : Component, IInteractive
    {
        /// <summary>
        /// Object that will be followed
        /// </summary>
        private GameObject _target;

        /// <summary>
        /// Object that will be followed
        /// </summary>
        public GameObject Target
        {
            get => _target;
            set
            {
                if (null == value || gameObject.Equals(value))
                    throw new System.ArgumentException(
                        "Target cannot be set to itself or to null!\nSet another object instead!");
                _target = value;
            }
        }

        /// <summary>
        /// Distance at which the object will start following the target.
        /// </summary>
        private float _interactiveDistance;

        /// <summary>
        /// Distance at which the object will start following the target.
        /// </summary>
        public float InteractionDistance => _interactiveDistance;

        /// <summary>
        /// Multiplier used for computing distance of interaction
        /// </summary>
        private float _distanceMultiplier;

        /// <summary>
        /// Multiplier used for computing distance of interaction
        /// </summary>
        public float DistanceMultiplier
        {
            get => _distanceMultiplier;
            set
            {
                if (1.0f > value)
                    throw new System.ArgumentException(
                        "Invalid argument!\nDistance multiplier must be greater or equal to 1.0f\nOtherwise you'd have caused a bug of entering into collider to be able to interact with.");
                _distanceMultiplier = value;
                _interactiveDistance = gameObject.GetComponent<SphereColliderComponent>().boundingSphere.Radius *
                                       _distanceMultiplier;
            }
        }

        /// <summary>
        /// Flag defining if <see cref="Target"/> is inside trigger zone. 
        /// </summary>
        private bool _triggered = false;
        
        /// <summary>
        /// Flag defining if <see cref="Target"/> is inside trigger zone. 
        /// </summary>
        public bool Triggered => _triggered;

        /// <summary>
        /// Flag defining if Self and <see cref="Target"/> reached each other.
        /// </summary>
        private bool _approached = false;
        
        /// <summary>
        /// Flag defining if Self and <see cref="Target"/> reached each other.
        /// </summary>
        public bool Approached => _approached;
        
        /// <summary>
        /// <see cref="Follower"/> component that will be used for following <see cref="Target"/>.
        /// </summary>
        private Follower _follower;
        
        public FollowComponent(GameObject self, GameObject target, float distanceMultiplier = 10f)
        {
            gameObject = self;
            if (null == gameObject.GetComponent<SphereColliderComponent>())
            {
                throw new System.ArgumentException("Game object must contain SphereColliderComponent");
            }
            // Create a new Follower component if it doesn't exist yet.
            _follower = gameObject.GetComponent<Follower>();
            if (null == _follower)
            {
                gameObject.AddComponent(new Follower(gameObject));
                _follower = gameObject.GetComponent<Follower>();
            }
            _distanceMultiplier = distanceMultiplier;
            _target = target;
        }

        public override void Start()
        {
            
            _interactiveDistance = gameObject.GetComponent<SphereColliderComponent>().boundingSphere.Radius *
                                   _distanceMultiplier;
        }

        public override void Update()
        {
            _approached = TargetAndSelfReached();
            if ((SquaredDistanceBetweenTargetAndSelf() <= (_interactiveDistance * _interactiveDistance)) && !_triggered)
            {
                if (!_approached)
                {
                    Pick();
                }
            }
        }

        private bool TargetAndSelfReached()
        {
            Console.WriteLine("SocialDistance:" + _follower.SocialDistance);
            Console.WriteLine("Distance:" + SquaredDistanceBetweenTargetAndSelf());
            Console.WriteLine("Enemy Approached? " + (SquaredDistanceBetweenTargetAndSelf()<=
                              (_follower.SocialDistance * _follower.SocialDistance)));
            return SquaredDistanceBetweenTargetAndSelf() <=
                   (_follower.SocialDistance * _follower.SocialDistance);
        }

        public void Pick()
        {
            _follower.Target = _target;
            _triggered = true;
            _approached = false;
        }

        public void Release()
        {
            _follower.Target = null;
            _triggered = false;
            _approached = false;
        }

        public float SquaredDistanceBetweenTargetAndSelf()
        {
            return Vector3.DistanceSquared(_target.transform.position, gameObject.transform.position);
        }
    }
}