namespace SolidSilnique.Core.Interfaces
{
    /// <summary>
    /// Interface for objects that can be interacted with
    /// </summary>
    public interface IInteractive
    {
        /// <summary>
        /// Interact with an object
        /// </summary>
        void Pick();
        /// <summary>
        /// Stop interacting with an object
        /// </summary>
        void Release();
        /// <summary>
        /// Counts squared distance between self and target
        /// </summary>
        /// <returns>distance between Self and Target. Squared distance is return to improve performance</returns>
        float SquaredDistanceBetweenTargetAndSelf();
    }
}