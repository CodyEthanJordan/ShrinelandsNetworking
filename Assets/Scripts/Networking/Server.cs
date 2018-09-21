using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public class Server : MonoBehaviour
    {
        public bool AuthorotativeServer;
        public float MessageTimer = 0;

        public readonly int maxConnections = 5;

        int reliableChannelID;
        int socketID;
        int socketPort = 8888;
        int connectionID;

        private void Start()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            if(AuthorotativeServer)
            {
                socketID = NetworkTransport.AddHost(topology, socketPort);
            }
            else
            {
                socketID = NetworkTransport.AddHost(topology, socketPort + 1);
            }
            Debug.Log("Socket Open. SocketId is: " + socketID);
        }

        public void SendSocketMessage()
        {
            byte error;
            byte[] buffer = new byte[1024];
            Stream stream = new MemoryStream(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, "HelloServer");

            int bufferSize = 1024;

            NetworkTransport.Send(socketID, connectionID, reliableChannelID, buffer, bufferSize, out error);
        }

        public void Connect()
        {
            byte error;
            connectionID = NetworkTransport.Connect(socketID, "127.0.0.1", socketPort, 0, out error);
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
                    break;
                case NetworkEventType.DataEvent:
                    Stream stream = new MemoryStream(recBuffer);
                    BinaryFormatter formatter = new BinaryFormatter();
                    string message = formatter.Deserialize(stream) as string;
                    Debug.Log("incoming message event received: " + message);
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("remote client event disconnected");
                    break;
            }
        }
    }
}
