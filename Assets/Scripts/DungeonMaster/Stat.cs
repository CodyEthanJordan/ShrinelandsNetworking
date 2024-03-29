﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Stat
    {
        private int _current;
        public int Current
        {
            get { return _current; }
            set { _current = Math.Max(0, Math.Min(value, Max)); }
        }
        public int Max;

        public Stat(int current, int max)
        {
            this._current = current;
            this.Max = max;
        }

        public override string ToString()
        {
            return Current + "/" + Max;
        }
    }
}
