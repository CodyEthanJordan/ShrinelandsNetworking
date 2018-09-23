using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Networking
{
    public class ClientInfo
    {
        public string Name;
        public int ConnectionID;
        public int HostID;
        public List<Guid> ControlledSides;

        public ClientInfo(string name, int connectionID, int hostID, List<Guid> controlledSides)
        {
            this.Name = name;
            this.ConnectionID = connectionID;
            this.HostID = hostID;
            this.ControlledSides = new List<Guid>(controlledSides);
        }
    }
}
