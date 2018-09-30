using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Networking
{
    public class NetworkMessage
    {
        public string Type;
        public object Contents;

        public NetworkMessage(string type, object contents)
        {
            this.Type = type;
            this.Contents = contents;
        }
    }
}
