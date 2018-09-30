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
                    if (connectedClients.Count > 0)
                    {
                        Debug.LogError("Yo who else is playing rn");
                    }
                    else
                    {
                        var allSides = battle.sides.Select(s => s.ID).ToList(); //set client to control all sides
                        connectedClients.Add(new ClientInfo(recConnectionId, recHostId));
                    }
                    break;
                case NetworkEventType.DataEvent:
                    var json = Unzip(recBuffer);
                    var message = JsonConvert.DeserializeObject<NetworkMessage>(json);
                    HandleClientMessage(message, recConnectionId, recHostId);

                    break;
                case NetworkEventType.DisconnectEvent:
                    var clientLost = connectedClients.First(c => c.HostID == recHostId && c.ConnectionID == recConnectionId);
                    connectedClients.Remove(clientLost);
                    Debug.Log("remote client event disconnected");
                    break;
            }
        }

        Dictionary<Side, PlayerInfo> WhosPlayingWhatSide()
        {
            var whosWho = new Dictionary<Side, PlayerInfo>();

            foreach (var side in battle.sides)
            {
                var playedBy = connectedClients.FirstOrDefault(c => c.Player.PlayingAsSideIDs.Any(s => s == side.ID));
                if (playedBy == null)
                {
                    whosWho.Add(side, null);
                }
                else
                {
                    whosWho.Add(side, playedBy.Player);
                }
            }

            return whosWho;
        }

        private void HandleClientMessage(NetworkMessage message, int recConnectionId, int recHostId)
        {
            switch (message.Type)
            {
                case "join game":
                    PlayerInfo info = JsonConvert.DeserializeObject<PlayerInfo>(message.JsonContents);
                    connectedClients.First(c => c.ConnectionID == recConnectionId).Player = info;
                    SendBattleInfo(recHostId, recConnectionId);
                    break;
                case "what sides":
                    var whosWho = WhosPlayingWhatSide();
                    SendMessageToClient(recHostId, recConnectionId, new NetworkMessage("sides", JsonConvert.SerializeObject(whosWho)));
                    break;

            }
        }

        private void HandleRequest(Request request, int clientHostID, int clientConnectionID)
        {
            List<Result> results = null;
            switch (request.Type)
            {
                case "Join Game":
                    SendBattleInfo(clientHostID, clientConnectionID);
                    break;
                case "Move":
                    results = battle.MoveUnit(request.Unit, request.Target);
                    break;
            }

            if (results != null && results.Count > 0)
            {
                TellClientsAboutResult(results);
            }
        }

        private void TellClientsAboutResult(List<Result> results)
        {

            //TODO: make in to generic function to send to all clients
            var message = new NetworkMessage("results", JsonConvert.SerializeObject(results));

            foreach (var client in connectedClients)
            {
                SendMessageToClient(client.HostID, client.ConnectionID, message);
            }
        }

        private void SendMessageToClient(int hostID, int connectionID, NetworkMessage message)
        {
            byte error;
            var json = JsonConvert.SerializeObject(message);
            var bytes = Zip(json);
            NetworkTransport.Send(hostID, connectionID, reliableChannelID, bytes, bytes.Length, out error);

            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogError("Network error when sending: " + (NetworkError)error);
            }
        }

        private void SendBattleInfo(int hostID, int connectionID)
        {
            var message = new NetworkMessage("load map", JsonConvert.SerializeObject(battle));
            SendMessageToClient(hostID, connectionID, message);
        }


    }
}
