using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Stat
    {
        public int Current;
        public int Max;

        Stat(int current, int max)
        {
            this.Current = current;
            this.Max = max;
        }
    }
}
