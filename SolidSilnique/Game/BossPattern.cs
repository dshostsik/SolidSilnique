using System;
using Microsoft.Xna.Framework;
using SolidSilnique.Core;
using SolidSilnique.Core.Components;

namespace SolidSilnique.Game
{
    internal enum BossPatternType
    {
        Line,
        Cross,
        Star
    }
    
    public class BossPattern
    {
        /// <summary>
        /// <see cref="GameObject"/> representing boss
        /// </summary>
        private GameObject _boss;
        
        /// <summary>
        /// Field of view of the boss.
        /// </summary>
        private BoundingSphere _bossArea;
        
        /// <summary>
        /// Object for generating random numbers.
        /// </summary>
        private static Random _random = new Random();
        
        // TODO: Deprecate??
        /// <summary>
        /// Health points of the boss
        /// </summary>
        private int _bossHP;
        /// <summary>
        /// Patterns of damage fields. Chosen randomly. 
        /// </summary>
        private readonly char[] _patterns;
        /// <summary>
        /// Bounding boxes that are generated according to the pattern.
        /// </summary>
        private BoundingBox[][] _damageBoxes;
        
        /// <summary>
        /// <see cref="GameObject"/> representing player./>
        /// </summary>
        private readonly GameObject _player;

        public BossPattern(GameObject boss, GameObject player)
        {
            _boss = boss ?? throw new ArgumentNullException(nameof(boss) + " object instance reference is null");
            _bossArea = boss.GetComponent<SphereColliderComponent>().boundingSphere;
            _bossHP = 100;
            
            _patterns = new char[3];

            _patterns[0] = '-';
            _patterns[1] = '+';
            _patterns[2] = '*';
            
            _damageBoxes = new BoundingBox[3][];
            
            // Demonstrative values so far. Idk what values should be here and how to rotate those fucking boxes
            _damageBoxes[0] = new BoundingBox[1];
            _damageBoxes[0][0] = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
            
            _damageBoxes[1] = new BoundingBox[2];
            _damageBoxes[1][0] = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
            _damageBoxes[1][1] = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
            
            _damageBoxes[2] = new BoundingBox[3];
            _damageBoxes[2][0] = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
            _damageBoxes[2][1] = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
            _damageBoxes[2][2] = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
            
            _player = player;
        }

        /// <summary>
        /// Checks if the player is in the range of boss.
        /// </summary>
        /// <returns></returns>
        private bool IsPlayerInBossRange()
        {
            return _bossArea.Intersects(_player.GetComponent<SphereColliderComponent>().boundingSphere);
        }

        /// <summary>
        /// Get bounding boxes according to the pattern.
        /// </summary>
        /// <param name="pattern"> pattern of the boxes representing damage fields</param>
        /// <returns>Array of boxes in certain positions and rotated to specific angles</returns>
        /// <exception cref="ArgumentException"> if something goes wrong and for some reason wrong pattern was passed</exception>
        private BoundingBox[] GetDamageBoxes(char pattern)
        {
            switch  (pattern)
            {
                case '-':
                        return _damageBoxes[0];
                case '+':
                        return _damageBoxes[1];
                case '*':
                        return _damageBoxes[2];
                default:
                        throw new ArgumentException("Invalid pattern. Something went wrong. Check GetDamageBoxes(char pattern) method.");
            }
        }
        
        /// <summary>
        /// Gets a random pattern for extracting damage boxes.
        /// </summary>
        /// <returns>Char from the <see cref="_patterns"/> array</returns>
        private char GetRandomAttackPattern()
        {
            return _patterns[_random.Next(0, 3)];
        }

        /// <summary>
        /// Just beat the fucking player!!!
        /// </summary>
        public void BeatPlayer()
        {
            if (IsPlayerInBossRange())
            {
                BoundingBox[] boxes = GetDamageBoxes(GetRandomAttackPattern());

                for (int i = 0; i < boxes.Length; i++)
                {
                    if (boxes[i].Intersects(_player.GetComponent<SphereColliderComponent>().boundingSphere)) Console.WriteLine("Player is in range of boss and they're beaten. Add player's HP logice please");
                }
            }
        }
    }
}