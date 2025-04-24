using System;

namespace SolidSilnique
{
    /// <summary>
    /// Custom exception for indicating uniform issues
    /// </summary>
    public class UniformNotFoundException : Exception
    {
        public UniformNotFoundException(string guiltyUniform, string message) : base(guiltyUniform + message)
        {
        }
    }
}