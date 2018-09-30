using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Networking
{
    public class NetworkMessage
    {
        public string Type;
        public string JsonContents;

        public NetworkMessage(string type, string contents)
        {
            this.Type = type;
            this.JsonContents = contents;
        }
    }
}
