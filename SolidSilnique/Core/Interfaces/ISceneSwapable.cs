using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Interfaces
{
    public interface ISceneSwapable<T>
    {
        T CurrentScene
        {
            get;
            set;
        }
    }
}
