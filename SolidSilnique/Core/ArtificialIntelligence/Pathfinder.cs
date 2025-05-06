#nullable enable
using Microsoft.Xna.Framework;

namespace SolidSilnique.Core.ArtificialIntelligence
{
    /// <summary>
    /// Class made for finding a path between two objects.
    /// </summary>
    public class Pathfinder
    {
        /// <summary>
        /// Reference to self - object that should find the path to target
        /// </summary>
        private GameObject self;

        /// <summary>
        /// Reference to target - object that self should reach
        /// </summary>
        private GameObject? target = null;

        /// <summary>
        /// Counted distance between self and target
        /// </summary>
        private float distance = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="self">reference to self - object that should find the path to target</param>
        public Pathfinder(GameObject self)
        {
            this.self = self;
        }

        /// <summary>
        /// Counts distance between self and target
        /// </summary>
        private void CountDistance()
        {
            if (target == null) return;

            distance = Vector3.Distance(self.transform.position, target.transform.position);
        }
    }
}