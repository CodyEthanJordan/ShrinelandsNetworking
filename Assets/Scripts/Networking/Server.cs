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
                    var json = Unzip(recBuffer);
                    Request clientRequest = JsonConvert.DeserializeObject<Request>(json);
                    HandleRequest(clientRequest, recHostId, recConnectionId);
                    
                    break;
                case NetworkEventType.DisconnectEvent:
                    var clientLost = connectedClients.First(c => c.HostID == recHostId && c.ConnectionID == recConnectionId);
                    connectedClients.Remove(clientLost);
                    Debug.Log("remote client event disconnected");
                    break;
            }
        }

        private void HandleRequest(Request request, int clientHostID, int clientConnectionID)
        {
            List<Result> results = null;
            switch(request.Type)
            {
                case "Join Game":
                    SendBattleInfo(clientHostID, clientConnectionID);
                    break;
                case "Move":
                    results = battle.MoveUnit(request.Unit, request.Target);
                    break;
            }

            if(results != null && results.Count > 0)
            {
                TellClientsAboutResult(results);
            }
        }

        private void TellClientsAboutResult(List<Result> results)
        {
            string resultJson = JsonConvert.SerializeObject(results);
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
