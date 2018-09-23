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
        public Camera Cam;
        public float Speed;
        public SmoothMouseLook mouseLook;


        [SerializeField] private Text serverIPText;
        [SerializeField] private GameObject connectionPanel;
        [SerializeField] private Client client;

        private List<BlockRenderer> blocks = new List<BlockRenderer>();

        private void Start()
        {
            client.OnConnected += Connected;
            client.OnRecieveBattle += InitializeBattle;

            mouseLook.enabled = false;
        }

        private void Update()
        {
            HandleCameraMovement();
        }

        private void HandleCameraMovement()
        {
            if (Input.GetMouseButtonDown(1))
            {
                mouseLook.enabled = true;
                Cursor.visible = false;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                mouseLook.enabled = false;
                Cursor.visible = true;
            }

            Cam.transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * Speed * Time.deltaTime, Space.Self);
            Cam.transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * Speed * Time.deltaTime, Space.Self);
            Cam.transform.Translate(Vector3.up * Input.GetAxis("Jump") * Speed * Time.deltaTime, Space.World);

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
                        newBlock.transform.position = new Vector3(i, k, j);
                        var br = newBlock.GetComponent<BlockRenderer>();
                        br.Become(battle.map.Blocks[i][j][k].Name);
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
