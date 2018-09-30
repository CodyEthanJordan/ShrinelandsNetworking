﻿using Assets.Scripts.DungeonMaster;
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
            var Message = new NetworkMessage("take action", moveRequest);
            SendToServer(Message);
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
                    connected = true;
                    PlayerInfo info = new PlayerInfo();
                    info.Name = PlayerName;
                    info.ID = ID;
                    SendToServer(new NetworkMessage("join game", info));
                    if(OnConnected != null)
                    {
                        OnConnected(this);
                    }
                    break;
                case NetworkEventType.DataEvent:
                    string message = Encoding.UTF8.GetString(recBuffer, 0, dataSize).Trim();
                    if(message.StartsWith("result"))
                    {
                        Result result = JsonConvert.DeserializeObject<Result>(message.Substring("result".Length + 1));
                        battle.HandleResult(result);
                    }
                    else
                    {
                        //assume its a serialized Battle for now
                        HandleData(recBuffer, dataSize);
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("remote client event disconnected");
                    connected = false;
                    break;
            }
        }

        private void HandleData(byte[] buffer, int dataSize)
        {
            RecievedBattle(buffer, dataSize);
        }

        private void RecievedBattle(byte[] buffer, int dataSize)
        {
            string battleJson = Unzip(buffer);

            this.battle = JsonConvert.DeserializeObject<Battle>(battleJson);

            if(OnRecieveBattle != null)
            {
                OnRecieveBattle(this, this.battle);
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
