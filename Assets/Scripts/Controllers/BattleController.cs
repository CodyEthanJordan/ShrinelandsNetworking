using Assets.Scripts.DungeonMaster;
using Assets.Scripts.Networking;
using Assets.Scripts.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class BattleController : MonoBehaviour
    {
        public GameObject BlockPrefab;

        [SerializeField] private Text serverIPText;
        [SerializeField] private GameObject connectionPanel;
        [SerializeField] private Client client;

        private List<BlockRenderer> blocks = new List<BlockRenderer>();

        private void Start()
        {
            client.OnConnected += Connected;
            client.OnRecieveBattle += InitializeBattle;
        }

        private void CleanUp()
        {
            foreach (var block in blocks)
            {
                Destroy(block.gameObject);
            }
            blocks.Clear();
        }

        private void InitializeBattle(object source, Battle battle)
        {
            CleanUp();

            for (int i = 0; i < battle.map.Shape[0]; i++)
            {
                for (int j = 0; j < battle.map.Shape[1]; j++)
                {
                    for (int k = 0; k < battle.map.Shape[2]; k++)
                    {
                        var newBlock = Instantiate(BlockPrefab, this.transform, true);
                        newBlock.transform.position = new Vector3(i, j, k);
                        var br = newBlock.GetComponent<BlockRenderer>();
                        br.Become(battle.map.Blocks[i][k][j].Name);
                        blocks.Add(br);
                    }
                }
            }
        }

        private void Connected(object source)
        {
            connectionPanel.SetActive(false); //don't need connection buttons now that we are connected
        }

        public void ConnectToServer()
        {
            client.ConnectToServer(serverIPText.text);
        }
    }
}
