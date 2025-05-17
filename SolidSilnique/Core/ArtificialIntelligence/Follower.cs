#nullable enable

using System;
using Microsoft.Xna.Framework;
using SolidSilnique.Core.Components;
using SolidSilnique.Core.Diagnostics;

namespace SolidSilnique.Core.ArtificialIntelligence
{
    /// <summary>
    /// Class that allows <see cref="GameObject"/> to follow a specified target.
    /// </summary>
    public class Follower : Component
    {
        /// <summary>
        /// Reference to the self-<see cref="GameObject"/>
        /// </summary>
        private readonly GameObject _self;

        /// <summary>
        /// Reference to a target <see cref="GameObject"/> that will be followed by <see cref="Self"/>
        /// </summary>
        private GameObject? _target;

        /// <summary>
        /// Multiplier for the distance between <see cref="Self"/> and <see cref="Target"/>. The default value is 1.5f
        /// </summary>
        private float _socialDistanceMultiplier;

        /// <summary>
        /// The distance between <see cref="Self"/> and <see cref="Target"/> in power of two. The default value is calculated based on <see cref="_socialDistanceMultiplier"/>
        /// </summary>
        private float _socialDistanceSquared;

        /// <summary>
        /// Reference to the self-<see cref="GameObject"/>
        /// </summary>
        public GameObject Self => _self;


        /// <summary>
        /// Calculates <see cref="SocialDistanceSquared"/> depending on <see cref="SocialDistanceMultiplier"/> and <see cref="Target"/>'s radius.
        /// </summary>
        /// <returns>Calculated length of radius in power of two defining private zone of target</returns>
        private float ComputeSocialDistanceSquared()
        {
            if (null == _target) return 0.0f;
            float output = _target.GetComponent<SphereColliderComponent>().boundingSphere.Radius;
            output *= output;
            output *= _socialDistanceMultiplier;
            return output;
        }
        
        /// <summary>
        /// Reference to a target <see cref="GameObject"/> that will be followed by <see cref="Self"/>.<p>Default value is <c>null</c>. Make sure that you set it before calling <see cref="GetFollowDirectionVector"/> method.</p>
        /// </summary>
        public GameObject? Target
        {
            get => _target;
            set
            {
                if (null != value && _self.Equals(value)) throw new ArgumentException("Target cannot be set to itself.\nSet another object instead!");
                _target = value;
                if (null != _target) _socialDistanceSquared = ComputeSocialDistanceSquared();
            }
        }

        /// <summary>
        /// Multiplier for the distance between <see cref="Self"/> and <see cref="Target"/>. The default value is 1.5f
        /// <p>Pay attention that if <see cref="Target"/> is not <c>null</c> then its social distance will be recalculated as soon as you set new value</p>
        /// </summary>
        public float SocialDistanceMultiplier
        {
            get => _socialDistanceMultiplier;
            set
            {
                if (1.0f > value)
                    throw new ArgumentException(
                        "Invalid argument!\nMultiplier must be at least 1.0f\nOtherwise you'd have caused a bug of infinite following interrupted by collisions");
                _socialDistanceMultiplier = value;
                if (null == _target) return;
                _socialDistanceSquared = ComputeSocialDistanceSquared();
            }
        }

        /// <summary>
        /// The distance between <see cref="Self"/> and <see cref="Target"/> in power of two. The default value is calculated based on <see cref="SocialDistanceMultiplier"/>
        /// <p>Pay attention that if <see cref="Target"/> is not <c>null</c> then you'll not be able to set that distance shorter that radius of its <see cref="SphereColliderComponent"/></p>
        /// </summary>
        public float SocialDistanceSquared
        {
            get => _socialDistanceSquared;
            set
            {
                if (null == _target) throw new TargetNotSetException("Target is null.\nMaybe you forgot to set it?");
                float colliderRadius = _target.GetComponent<SphereColliderComponent>().boundingSphere.Radius;
                if (value < colliderRadius * colliderRadius)
                    throw new ArgumentException(
                        "Invalid argument!\nSocial distance must be greater or equal to radius, try again!\nOtherwise you'd have caused a bug of infinite following interrupted by collisions");
                _socialDistanceSquared = value;
            }
        }

        /// <summary>
        /// Constructor.<p>Pay attention that <see cref="Target"/>'s default value is <c>null</c> and must be set before calling <see cref="GetFollowDirectionVector"/> method."/></p>
        /// </summary>
        /// <param name="self">A self-object that should follow <see cref="Target"/></param>
        /// <param name="socialDistanceMultiplier">Multiplier for the distance between <see cref="Self"/> and <see cref="Target"/>. The default value is 1.5f</param>
        public Follower(GameObject self, float socialDistanceMultiplier = 1.5f)
        {
            _self = self;
            _socialDistanceMultiplier = socialDistanceMultiplier;
        }

        /// <summary>
        /// Returns the direction of speed of following.<p>Make sure that <see cref="Target"/> is set before calling this method.</p>
        /// <returns><c>direction</c> vector if <see cref="Target"/> is not <c>null</c>, else - <c>Vector3.Zero</c></returns>
        /// <exception cref="TargetNotSetException"> if <see cref="Target"/> is null</exception>
        /// </summary>
        public Vector3 GetFollowDirectionVector()
        {
            if (null == _target) throw new TargetNotSetException("Target is null.\nMaybe you forgot to set it?");

            Vector3 direction = _target.transform.position - _self.transform.position;
            if (direction.LengthSquared() <= _socialDistanceSquared) return Vector3.Zero;
            direction.Normalize();

            direction.Y = 0.0f;
            
            return direction;
        }

        public override void Start()
        {
            
        }
        
        public override void Update()
        {
            gameObject.transform.position += GetFollowDirectionVector() * Time.deltaTime * 10;
        }
    }
}