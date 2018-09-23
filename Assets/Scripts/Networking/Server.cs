using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.DungeonMaster;
using Newtonsoft.Json;
using System.IO.Compression;

namespace Assets.Scripts.Networking
{
    public class Server : NetworkManager
    {
        public readonly int maxConnections = 5;

        private int socketPort = 8888;
        private int connectionID;

        private void Start()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, socketPort);
            Debug.Log("Socket Open. hostID is: " + hostID);

            //create debug battle
            battle = Battle.GetDebugBattle();
       }

        public void Connect()
        {
            byte error;
            connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", socketPort, 0, out error);
            Debug.Log("Connected to server. ConnectionId: " + connectionID);
        }

        void Update()
        {
            int recHostId;
            int recConnectionId;
            int recChannelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            byte error;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId,
                out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    Debug.Log("incoming connection event received");
                    SendInfo(recHostId, recConnectionId);
                    break;
                case NetworkEventType.DataEvent:
                    Stream stream = new MemoryStream(recBuffer);
                    BinaryFormatter formatter = new BinaryFormatter();
                    string message = formatter.Deserialize(stream) as string;
                    Debug.Log("incoming message event received: " + message);
                    if(message == "heartbeat")
                    {
                        Debug.Log("Hey " + recHostId + " is still alive");
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("remote client event disconnected");
                    break;
            }
        }

        void SendInfo(int hostID, int connectionID)
        {
            byte error;
            string json = JsonConvert.SerializeObject(battle, Formatting.None);
            var bytes = Zip(json);

            NetworkTransport.Send(hostID, connectionID, reliableChannelID, bytes, bytes.Length, out error);
        }

        
    }
}
