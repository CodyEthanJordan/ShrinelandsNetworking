using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Networking
{
    public class ClientInfo
    {
        public int ConnectionID;
        public int HostID;
        public PlayerInfo Player;

        public ClientInfo(int connectionID, int hostID)
        {
            this.ConnectionID = connectionID;
            this.HostID = hostID;
        }
    }
}
