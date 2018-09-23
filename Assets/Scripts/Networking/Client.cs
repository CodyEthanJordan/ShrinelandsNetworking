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

        private float heartbeatTimer;
        private float heartbeatRate = 3f;
        private bool connected = false;

        private void Start()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(config, 1);
            hostID = NetworkTransport.AddHost(topology, serverSocketPort + 1);
        }

        public void ConnectToServer()
        {
            byte error;
            string serverIP = "192.168.1.3";
            connectionID = NetworkTransport.Connect(hostID, "192.168.1.3", serverSocketPort, 0, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogError("Networking error : " + (NetworkError)error);
            }
            Debug.Log("Requesting connection from server " + serverIP + ". ConnectionId: " + connectionID);
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
            var buffer = Encoding.UTF8.GetBytes(message);
            var bufferLength = Encoding.UTF8.GetByteCount(message);

            NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, bufferLength, out error);

            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogError("Networking error : " + (NetworkError)error);
            }
        }

        private void RequestBattle()
        {
            SendToServer("send battle");
        }

        private void Update()
        {
            if(connected)
            {
                Heartbeat();
            }

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
                    Debug.Log("incoming connection event received");
                    RequestBattle();
                    connected = true;
                    break;
                case NetworkEventType.DataEvent:
                    HandleData(recBuffer, dataSize);
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("remote client event disconnected");
                    connected = false;
                    break;
            }
        }

        private void HandleData(byte[] buffer, int dataSize)
        {
            string bat = Unzip(buffer);
            Debug.Log(bat);
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
