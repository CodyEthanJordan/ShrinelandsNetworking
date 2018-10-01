using Assets.Scripts.DungeonMaster;
using Newtonsoft.Json;
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
        public event ConnectionEvent OnConnected;
        public event BattleRecieved OnRecieveBattle;
        public event SidesRecieved OnRecieveSides;
        public event StartPlayAs OnStartPlay;

        private int connectionID;

        private float heartbeatTimer;
        private float heartbeatRate = 3f;
        private bool connected = false;

        public string PlayerName;
        public Guid ID;

        private void Start()
        {
            if (PlayerPrefs.HasKey("PlayerID"))
            {
                ID = new Guid(PlayerPrefs.GetString("PlayerID"));
            }
            else
            {
                ID = Guid.NewGuid();
                PlayerPrefs.SetString("PlayerID", ID.ToString());
                PlayerPrefs.Save();
            }

            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(config, 1);
            hostID = NetworkTransport.AddHost(topology, serverSocketPort + 1);
        }

        internal void MoveUnit(Guid unitRepresented, Vector3Int position)
        {
            var moveRequest = Request.ToMove(unitRepresented, position);
            var Message = new NetworkMessage("take action",
                JsonConvert.SerializeObject(moveRequest));
            SendToServer(Message);
        }

        internal void PlayAs(List<Guid> playAs)
        {
            var message = new NetworkMessage("play as",
                JsonConvert.SerializeObject(playAs));
            SendToServer(message);
        }

        public void ConnectToServer(string address, string name)
        {
            this.PlayerName = name;

            byte error;
            connectionID = NetworkTransport.Connect(hostID, address, serverSocketPort, 0, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogError("Networking error : " + (NetworkError)error);
            }
            Debug.Log("Requesting connection from server " + address + ". ConnectionId: " + connectionID);
        }

        public void SendToServer(NetworkMessage message)
        {
            SendToServer(JsonConvert.SerializeObject(message));
        }

        public void SendToServer(string message)
        {
            byte error;
            var compressedMessage = Zip(message);

            NetworkTransport.Send(hostID, connectionID, reliableChannelID, compressedMessage, compressedMessage.Length, out error);

            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogError("Networking error : " + (NetworkError)error);
            }
        }

        private void Update()
        {
            if (connected)
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
                    connected = true;
                    PlayerInfo info = new PlayerInfo();
                    info.Name = PlayerName;
                    info.ID = ID;
                    SendToServer(new NetworkMessage("join game",
                        JsonConvert.SerializeObject(info)));
                    if (OnConnected != null)
                    {
                        OnConnected(this);
                    }
                    break;
                case NetworkEventType.DataEvent:
                    var json = Unzip(recBuffer);
                    var message = JsonConvert.DeserializeObject<NetworkMessage>(json);
                    HandleMessageFromServer(message);
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("remote client event disconnected");
                    connected = false;
                    break;
            }
        }

        private void HandleMessageFromServer(NetworkMessage message)
        {
            switch (message.Type)
            {
                case "load map":
                    this.battle = JsonConvert.DeserializeObject<Battle>(message.JsonContents);
                    if (OnRecieveBattle != null)
                    {
                        OnRecieveBattle(this, this.battle);
                    }

                    var askAboutSides = new NetworkMessage("what sides", null);
                    SendToServer(askAboutSides);
                    break;
                case "sides":
                    var whosWho = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(message.JsonContents);
                    if (OnRecieveSides != null)
                    {
                        OnRecieveSides(this, whosWho);
                    }
                    break;
                case "new player": //TODO: maybe need new player for "player is connected" message and specific call to start game
                    var playerInfo = JsonConvert.DeserializeObject<List<PlayerInfo>>(message.JsonContents);
                    var me = playerInfo.First(p => p.ID == this.ID);
                    if (OnStartPlay != null)
                    {
                        OnStartPlay(this, me.PlayingAsSideIDs);
                    }
                    break;
                case "results":
                    var results = JsonConvert.DeserializeObject<List<Result>>(message.JsonContents);
                    battle.HandleResults(results);
                    break;

            }
        }

        private void Heartbeat()
        {
            heartbeatTimer += Time.deltaTime;
            if (heartbeatTimer > heartbeatRate)
            {
                heartbeatTimer = 0;
                var message = new NetworkMessage("heartbeat", null);
                SendToServer(message);
            }
        }
    }
}
