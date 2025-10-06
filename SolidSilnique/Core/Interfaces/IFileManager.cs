using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Interfaces
{
    /// <summary>
    /// Interface for FILE reader classes
    /// </summary>
    /// <typeparam name="T">Type that class reads and writes</typeparam>
    public interface IFileManager<T>
    {
        /// <summary>
        /// Method for reading data from the specified file
        /// </summary>
        /// <returns>Value of the specified type</returns>
        T Read();
        /// <summary>
        /// Method for writing data to file
        /// </summary>
        /// <param name="value">Value to write</param>
        void Write(T value);
    }
}
