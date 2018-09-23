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

        private List<ClientInfo> connectedClients = new List<ClientInfo>();

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
                    if(connectedClients.Count > 0)
                    {
                        Debug.LogError("Yo who else is playing rn");
                    }
                    else
                    {
                        var allSides = battle.sides.Select(s => s.ID).ToList(); //set client to control all sides
                        connectedClients.Add(new ClientInfo("DefaultName", recConnectionId, recHostId, allSides));
                    }
                    break;
                case NetworkEventType.DataEvent:
                    string message = Encoding.UTF8.GetString(recBuffer, 0, dataSize).Trim();
                    Debug.Log("incoming message event received: " + message);
                    if(message == "send battle")
                    {
                        Debug.Log("Serializing battle for client " + recHostId);
                        SendBattleInfo(recHostId, recConnectionId);
                    }
                    else if(message.StartsWith("request"))
                    {
                        HandleRequest(message.Substring("request".Length + 1));
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    var clientLost = connectedClients.First(c => c.HostID == recHostId && c.ConnectionID == recConnectionId);
                    connectedClients.Remove(clientLost);
                    Debug.Log("remote client event disconnected");
                    break;
            }
        }

        private void HandleRequest(string requestJson)
        {
            var request = JsonConvert.DeserializeObject<Request>(requestJson);
            Result result = null;
            switch(request.Type)
            {
                case "Move":
                    result = battle.MoveUnit(request.Unit, request.Target);
                    break;
            }

            if(result != null)
            {
                TellClientsAboutResult(result);
            }
        }

        private void TellClientsAboutResult(Result result)
        {
            string resultJson = JsonConvert.SerializeObject(result);
            byte[] message = Encoding.UTF8.GetBytes("result " + resultJson);

            foreach (var client in connectedClients)
            {
                SendMessageToClient(client.HostID, client.ConnectionID, message);
            }
        }

        private void SendMessageToClient(int hostID, int connectionID, byte[] message)
        {
            byte error;
            NetworkTransport.Send(hostID, connectionID, reliableChannelID, message, message.Length, out error);
            if((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogError("Network error when sending: " + (NetworkError)error);
            }
        }

        private void SendBattleInfo(int hostID, int connectionID)
        {
            string json = JsonConvert.SerializeObject(battle, Formatting.None);
            var bytes = Zip(json);
            SendMessageToClient(hostID, connectionID, bytes);
        }

        
    }
}
