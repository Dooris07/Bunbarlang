using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunbarlang
{
    public class Player
    {
        public string Name { get; set; }
        public int Chips { get; set; }

        public void AddChips(int amount)
        {
            Chips += amount;
        }

        public bool RemoveChips(int amount)
        {
            if (Chips < amount) return false;
            Chips -= amount;
            return true;
        }
    }

}
