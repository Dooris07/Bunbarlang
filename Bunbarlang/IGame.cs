using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunbarlang
{
    public interface IGame
    {
        string Name { get; }
        bool IsCompleted { get; }
        void Play(Player player);
    }

}
