using System;

namespace SolidSilnique.Core.Diagnostics
{
    /// <summary>
    /// Custom exception for indicating uniform issues
    /// </summary>
    public class  UniformNotFoundException(string guiltyUniform, string message) : Exception(guiltyUniform + message);
}