using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Side
    {
        public Guid ID { get; protected set; }
        public string Name { get; protected set; }

        public Side(string name)
        {
            ID = Guid.NewGuid();
            this.Name = name;
        }
    }
}
