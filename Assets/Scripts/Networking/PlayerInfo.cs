using Assets.Scripts.DungeonMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Networking
{
    public class PlayerInfo
    {
        public string Name;
        public Guid ID;
        public List<Guid> PlayingAsSideIDs;
        public Battle BattleInfo;
    }
}
