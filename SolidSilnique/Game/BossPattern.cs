using System;
using SolidSilnique.Core;

namespace SolidSilnique.Game
{
    public class BossPattern
    {
        private GameObject _boss;
        private int _bossHP;
        private readonly char[] _patterns;

        private GameObject _player;

        public BossPattern(GameObject boss, GameObject player)
        {
            _boss = boss;
            _bossHP = 100;
            _patterns = new char[3];

            _patterns[0] = '+';
            _patterns[1] = '-';
            _patterns[2] = '*';
            
            _player = player;
        }

        private bool IsPlayerInBossRange()
        {
            return false;
        }

        private char GetRandomAttackPattern()
        {
            int randomIndex = new Random().Next(0, 3);
            return _patterns[randomIndex];
        }

        public void BeatPlayer()
        {
            throw new NotImplementedException();
        }
    }
}