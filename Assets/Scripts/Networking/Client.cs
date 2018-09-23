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
    public class Client : NetworkManager
    {
        private int connectionID;
        private int serverSocketPort = 8888;

        private float heartbeatTimer;
        private float heartbeatRate = 0.5f;

        private void Start()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(config, 1);
            hostID = NetworkTransport.AddHost(topology, serverSocketPort + 1);
        }

        public void Connect()
        {
            byte error;
            connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", serverSocketPort, 0, out error);
            Debug.Log("Connected to server. ConnectionId: " + connectionID);
        }

        public void SendTestMessage()
        {
            byte error;
            byte[] buffer = new byte[1024];
            Stream stream = new MemoryStream(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, "HelloServer");

            int bufferSize = 1024;

            NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, bufferSize, out error);
        }

        private void SendToServer(string message)
        {
            byte error;
            byte[] buffer = new byte[message.Length];
            Stream stream = new MemoryStream(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, message);

            NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, message.Length, out error);

            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogError("Networking error : " + (NetworkError)error);
            }
        }

        private void RequestBattle()
        {
            SendToServer("request battle");
        }

        private void Update()
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
                    Debug.Log("incoming message event received:  " + message);
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("remote client event disconnected");
                    break;
            }
        }

        private void Heartbeat()
        {
            heartbeatTimer += Time.deltaTime;
            if(heartbeatTimer > heartbeatRate)
            {
                heartbeatTimer = 0;
                SendToServer("heartbeat");
            }
        }
    }
}
