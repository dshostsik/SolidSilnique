#nullable enable

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolidSilnique.Core.Components;
using SolidSilnique.Core.Diagnostics;

namespace SolidSilnique.Core.ArtificialIntelligence
{

    public enum AIState {
        HOSTILE_PATROL,
        HOSTILE_CHASE,
        HOSTILE_RETURN,
        FRIENDLY_FOLLOW,
        FRIENDLY_LOST
    
    }





    /// <summary>
    /// Class that allows <see cref="GameObject"/> to follow a specified target.
    /// </summary>
    public class Follower : Component
    {

        public static GameObject enemyToFight = null;



        /// <summary>
        /// Reference to a target <see cref="GameObject"/> that will be followed by Self
        /// </summary>
        private GameObject? _target;

        /// <summary>
        /// Multiplier for the distance between Self and <see cref="Target"/>. The default value is 1.5f
        /// </summary>
        private float _socialDistanceMultiplier;

        public AIState state = AIState.HOSTILE_PATROL;
        public Vector3 spawnPoint;

        //Patrol State
        public float patrolTimer = 5f;
        public float patrolTimeToChange = 0f;
        public Vector3 patrolTargetOffset = Vector3.Zero;
        public float patrolRadius = 20f;
        public Random patrolRandom = new Random();
        public Vector3 moveVec = Vector3.Zero;

        //Chase State
        public float aggroRange = 10f;
        public float homeRange = 30f;

        //Lost State
        public float teleportRange = 30f;



        /// <summary>
        /// The distance between Self and <see cref="Target"/> . The default value is calculated based on <see cref="_socialDistanceMultiplier"/>
        /// </summary>
        private float _socialDistance;

        /// <summary>
        /// Reference to a target <see cref="GameObject"/> that will be followed by Self.<p>Default value is <c>null</c>. Make sure that you set it before calling <see cref="GetFollowDirectionVector"/> method.</p>
        /// </summary>
        public GameObject? Target
        {
            get => _target;
            set
            {
                if (null == value /*&& this.gameObject.Equals(value)*/)
                    throw new ArgumentException("Target cannot be set to itself.\nSet another object instead!");
                _target = value;
                if (null == _target)
                {
                    _socialDistance = 0.0f;
                    return;
                }
                var collider = _target.GetComponent<SphereColliderComponent>() ?? throw new ArgumentException("Game object must contain SphereColliderComponent");
                _socialDistance = collider.boundingSphere.Radius *
                                  _socialDistanceMultiplier;
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
                var collider = _target.GetComponent<SphereColliderComponent>() ?? throw new ArgumentException("Game object must contain SphereColliderComponent");
                _socialDistance = collider.boundingSphere.Radius *
                                  _socialDistanceMultiplier;
            }
        }

        /// <summary>
        /// The distance between Self and <see cref="Target"/> . The default value is calculated based on <see cref="SocialDistanceMultiplier"/>
        /// <p>Pay attention that if <see cref="Target"/> is not <c>null</c> then you'll not be able to set that distance shorter that radius of its <see cref="SphereColliderComponent"/></p>
        /// </summary>
        public float SocialDistance
        {
            get => _socialDistance;
            set
            {
                if (null == _target) throw new TargetNotSetException("Target is null.\nMaybe you forgot to set it?");
                if (value < _target.GetComponent<SphereColliderComponent>().boundingSphere.Radius)
                    throw new ArgumentException(
                        "Invalid argument!\nSocial distance must be greater or equal to radius, try again!\nOtherwise you'd have caused a bug of infinite following interrupted by collisions");
                _socialDistance = value;
            }
        }

        /// <summary>
        /// Constructor.<p>Pay attention that <see cref="Target"/>'s default value is <c>null</c> and must be set before calling <see cref="GetFollowDirectionVector"/> method."/></p>
        /// </summary>
        /// <param name="self">A self-object that should follow <see cref="Target"/></param>
        /// <param name="socialDistanceMultiplier">Multiplier for the distance between <see cref="Self"/> and <see cref="Target"/>. The default value is 1.5f</param>
        public Follower(GameObject self, float socialDistanceMultiplier = 1.5f)
        {
            gameObject = self;
            _socialDistanceMultiplier = socialDistanceMultiplier;
        }

        /// <summary>
        /// Returns the direction of speed of following.<p>Make sure that <see cref="Target"/> is set before calling this method.</p>
        /// <returns><c>direction</c> vector if <see cref="Target"/> is not <c>null</c>, else - <c>Vector3.Zero</c></returns>
        /// <exception cref="TargetNotSetException"> if <see cref="Target"/> is null</exception>
        /// </summary>
        public Vector3 GetFollowDirectionVector()
        {
            if (null == _target) return Vector3.Zero;

            Vector3 direction = _target.transform.position - this.gameObject.transform.position;
            direction.Y = 0.0f;
            if (direction.LengthSquared() <= (_socialDistance * _socialDistance)) return Vector3.Zero;

            direction.Normalize();

            return direction;
        }

        public override void Start()
        {
            spawnPoint = gameObject.transform.globalPosition;
            spawnPoint -= Vector3.Up * spawnPoint.Y - Vector3.Up * EngineManager.scene.environmentObject.GetHeight(spawnPoint);
            patrolTimeToChange = patrolTimer * (float)patrolRandom.NextDouble();
        }

        /// <summary>
        /// Moves <see cref="GameObject"/> towards <see cref="Target"/> with a direction from <see cref="GetFollowDirectionVector"/>
        /// </summary>
        public override void Update()
        {
            gameObject.transform.position += Vector3.Down * Time.deltaTime * 9.81f;

            if (enemyToFight != null) {
                return;
            }

            switch (state)
            {
                case AIState.HOSTILE_PATROL:
                    PatrolUpdate();
                    break;
                case AIState.HOSTILE_CHASE:
                    ChaseUpdate();
                    break;
                case AIState.HOSTILE_RETURN:
                    ReturnUpdate();
                    break;
				case AIState.FRIENDLY_FOLLOW:
					FollowUpdate();
					break;
				case AIState.FRIENDLY_LOST:
					TeleportUpdate();
					break;


			}


        }

        public void PatrolUpdate()
        {

            

            patrolTimeToChange += Time.deltaTime;
            if (patrolTimeToChange >= patrolTimer)
            {
                patrolTimeToChange -= patrolTimer;
                //Handle patrol point change
                patrolTargetOffset = (Vector3.Left * ((float)patrolRandom.NextDouble() * patrolRadius - patrolRadius / 2)) + (Vector3.Forward * ((float)patrolRandom.NextDouble() * patrolRadius - patrolRadius / 2));
            }

            //GO TO
            if (Vector3.DistanceSquared(spawnPoint + patrolTargetOffset, gameObject.transform.position) >= 0.25)
            {
                moveVec = -(gameObject.transform.position - (spawnPoint + patrolTargetOffset));
                moveVec.Normalize();

                gameObject.transform.position += moveVec * Time.deltaTime * 2;
            }


            //State Transition
            // -> Chase state
            if (Vector3.DistanceSquared(this.gameObject.transform.position, Target.transform.position) <= aggroRange * aggroRange)
            {
                state = AIState.HOSTILE_CHASE;
            }

        }

        public void ChaseUpdate()
        {
            gameObject.transform.position += GetFollowDirectionVector() * Time.deltaTime * 5;

            //State Transition
            // -> Return state
            if (Vector3.DistanceSquared(this.gameObject.transform.position, Target.transform.position) >= (aggroRange * 1.5) * (aggroRange * 1.5) || Vector3.DistanceSquared(this.gameObject.transform.position, spawnPoint) >= homeRange * homeRange)
            {
                state = AIState.HOSTILE_RETURN;
            }

            if (Vector3.DistanceSquared(this.gameObject.transform.position, Target.transform.position) <= SocialDistance*SocialDistance)
            {
                
                if(enemyToFight == null)
                {
                    enemyToFight = gameObject;
                }

                

			}

        }


        public void ReturnUpdate()
        {

            if ((Vector3.DistanceSquared(this.gameObject.transform.position, Target.transform.position) <= aggroRange * aggroRange * 0.75f * 0.75f))
            {
                //State Transition
                // -> Chase state
                state = AIState.HOSTILE_CHASE;
            }
            else if (Vector3.DistanceSquared(spawnPoint, gameObject.transform.position) >= homeRange * homeRange * 0.3f * 0.3f)
            {
                moveVec = -(gameObject.transform.position - (spawnPoint + patrolTargetOffset));
                moveVec.Normalize();

                gameObject.transform.position += moveVec * Time.deltaTime * 4;

            }
            else
            {
                //State Transition
                // -> Patrol state
                state = AIState.HOSTILE_PATROL;

            }
        }

        public void FollowUpdate()
        {
			gameObject.transform.position += GetFollowDirectionVector() * Time.deltaTime * 8;
			if ((Vector3.DistanceSquared(this.gameObject.transform.position, Target.transform.position) >= teleportRange * teleportRange))
			{
				//State Transition
				// -> Teleport state
				state = AIState.FRIENDLY_LOST;
			}
		}

		public void TeleportUpdate()
		{
			moveVec = -(gameObject.transform.position - Target.transform.position);
            gameObject.transform.position += moveVec * ((moveVec.Length() - SocialDistance) / moveVec.Length());
			//State Transition
			// -> Follow state
			state = AIState.FRIENDLY_FOLLOW;
		}

        public void SetFriendly() {
            //Tex change
            //State Transition
            // -> Follow state - turn friendly
            state = AIState.FRIENDLY_FOLLOW;
            gameObject.albedo = Color.White;


        }
	}
}