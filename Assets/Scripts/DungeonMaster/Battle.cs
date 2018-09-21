using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DungeonMaster
{
    public class Battle
    {
        public Guid ID;

        Battle()
        {
            ID = Guid.NewGuid();
        }

        static Battle CreateDefaultBattle()
        {

            return null;
        }
    }
}
