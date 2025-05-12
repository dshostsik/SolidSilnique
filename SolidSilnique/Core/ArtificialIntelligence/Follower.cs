#nullable enable

using Microsoft.Xna.Framework;
using SolidSilnique.Core.Components;
using SolidSilnique.Core.Diagnostics;

namespace SolidSilnique.Core.ArtificialIntelligence
{
    /// <summary>
    /// Class that allows <see cref="GameObject"/> to follow a specified target.
    /// </summary>
    public class Follower
    {
        /// <summary>
        /// Reference to the self-<see cref="GameObject"/>
        /// </summary>
        private GameObject _self;

        /// <summary>
        /// Reference to the self-object's <see cref="SphereColliderComponent"/>
        /// </summary>
        private SphereColliderComponent _selfZone;

        /// <summary>
        /// Reference to self-object's <see cref="BoundingSphere"/>
        /// </summary>
        private BoundingSphere _selfZoneSphere;

        /// <summary>
        /// Reference to a target <see cref="GameObject"/> that will be followed by <see cref="Self"/>
        /// </summary>
        private GameObject? _target;

        /// <summary>
        /// Reference to the target's <see cref="SphereColliderComponent"/>
        /// </summary>
        private SphereColliderComponent _targetZone;

        /// <summary>
        /// Reference to target's <see cref="BoundingSphere"/>
        /// </summary>
        private BoundingSphere _targetZoneSphere;

        /// <summary>
        /// Multiplier for the distance between <see cref="Self"/> and <see cref="Target"/>. The default value is 1.5f
        /// </summary>
        private float _socialDistance;

        /// <summary>
        /// Reference to the self-<see cref="GameObject"/>
        /// </summary>
        public GameObject Self => _self;

        /// <summary>
        /// Reference to a target <see cref="GameObject"/> that will be followed by <see cref="Self"/>.<p>Default value is <c>null</c>. Make sure that you set it before calling <see cref="Follow"/> method.</p>
        /// </summary>
        public GameObject Target
        {
            get => _target;
            set
            {
                _target = value;
                _targetZone =
                    new SphereColliderComponent(value.GetComponent<SphereColliderComponent>().boundingSphere.Radius *
                                                _socialDistance);
                _targetZone.isStatic = true;
                _targetZoneSphere = _targetZone.boundingSphere;
            }
        }

        /// <summary>
        /// Multiplier for the distance between <see cref="Self"/> and <see cref="Target"/>. The default value is 1.5f
        /// </summary>
        public float SocialDistance
        {
            get => _socialDistance;
            set => _socialDistance = value;
        }

        /// <summary>
        /// Constructor.<p>Pay attention that <see cref="Target"/>'s default value is <c>null</c> and must be set before calling <see cref="Follow"/> method."/></p>
        /// </summary>
        /// <param name="self">A self-object that should follow <see cref="Target"/></param>
        /// <param name="socialDistance">Multiplier for the distance between <see cref="Self"/> and <see cref="Target"/>. The default value is 1.5f</param>
        public Follower(GameObject self, float socialDistance = 1.5f)
        {
            _self = self;
            _selfZone = self.GetComponent<SphereColliderComponent>();
            _selfZoneSphere = _selfZone.boundingSphere;
            _socialDistance = socialDistance;
        }

        /// <summary>
        /// Returns the direction of speed of following.<p>Make sure that <see cref="Target"/> is set before calling this method.</p>
        /// <returns><c>direction</c> vector if <see cref="Target"/> is not <c>null</c>, else - <c>null</c></returns>
        /// <exception cref="TargetNotSetException"> if <see cref="Target"/> is null</exception>
        /// </summary>
        public Vector3 Follow()
        {
            if (null == _target) throw new TargetNotSetException("Target is null.\nMaybe you forgot to set it?");
            if (_selfZoneSphere.Intersects(_targetZoneSphere)) return Vector3.Zero;

            Vector3 direction = _target.transform.position - _self.transform.position;

            direction.Normalize();

            return direction;
        }
    }
}