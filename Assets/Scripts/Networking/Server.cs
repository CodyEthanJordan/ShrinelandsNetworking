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

        private int connectionID;

        private void Start()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, serverSocketPort);
            Debug.Log("Socket Open. hostID is: " + hostID);

            //create debug battle
            battle = Battle.GetDebugBattle();
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

            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogError("Networking error : " + (NetworkError)error);
            }

            switch (recNetworkEvent)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    Debug.Log("incoming connection event received from " + recHostId);
                    break;
                case NetworkEventType.DataEvent:
                    string message = Encoding.UTF8.GetString(recBuffer, 0, dataSize).Trim();
                    Debug.Log("incoming message event received: " + message);
                    if(message == "send battle")
                    {
                        Debug.Log("Serializing battle for client " + recHostId);
                        SendBattleInfo(recHostId, recConnectionId);
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("remote client event disconnected");
                    break;
            }
        }

        void SendBattleInfo(int hostID, int connectionID)
        {
            byte error;
            string json = JsonConvert.SerializeObject(battle, Formatting.None);
            var bytes = Zip(json);
            Debug.Log("Sending battle info to " + hostID);
            NetworkTransport.Send(hostID, connectionID, reliableChannelID, bytes, bytes.Length, out error);
        }

        
    }
}
