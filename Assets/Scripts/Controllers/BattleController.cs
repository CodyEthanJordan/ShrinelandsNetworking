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
        public GameObject UnitPrefab;
        public Camera Cam;

        public float Speed;
        public SmoothMouseLook mouseLook;

        public Transform UnitParent;
        public Transform BlockParent;

        public UnitRenderer SelectedUnit;
        public UnitInfoPanel unitInfoPanel;

        [SerializeField] private Text serverIPText;
        [SerializeField] private GameObject connectionPanel;
        [SerializeField] private Client client;

        private List<BlockRenderer> blocks = new List<BlockRenderer>();
        private List<UnitRenderer> units = new List<UnitRenderer>();
        private Animator FSM;

        private void Awake()
        {
            FSM = GetComponent<Animator>();
        }

        private void Start()
        {
            client.OnConnected += Connected;
            client.OnRecieveBattle += InitializeBattle;

            mouseLook.enabled = false;
        }

        private void Update()
        {
            HandleCameraMovement();
            HandleClickingStuff();
        }

        private void HandleClickingStuff()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                FSM.SetTrigger("Deselect");
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    if (hit.transform.gameObject.CompareTag("Unit"))
                    {
                        SelectedUnit = hit.transform.gameObject.GetComponent<UnitRenderer>();
                        FSM.SetTrigger("UnitClicked");
                    }
                    else
                    {
                        FSM.SetTrigger("Deselect"); //TODO: let FSM handle this stuff
                    }
                }
            }
        }

        public void ShowUnitStats(Guid unitRepresented)
        {
            unitInfoPanel.gameObject.SetActive(true);
            unitInfoPanel.ShowUnit(client.battle.units.First(u => u.ID == unitRepresented));
        }

        public void HideUnitStats()
        {
            unitInfoPanel.gameObject.SetActive(false);
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

            foreach (var unit in units)
            {
                Destroy(unit.gameObject);
            }
            units.Clear();
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
                        var newBlock = Instantiate(BlockPrefab, BlockParent, true);
                        newBlock.transform.position = new Vector3(i, k, j);
                        var br = newBlock.GetComponent<BlockRenderer>();
                        br.Become(battle.map.Blocks[i][j][k].Name);
                        blocks.Add(br);
                    }
                }
            }

            foreach (var unit in battle.units)
            {
                var newUnit = Instantiate(UnitPrefab, UnitParent, true);
                newUnit.transform.position = new Vector3(unit.Position.x, unit.Position.z, unit.Position.y);
                var ur = newUnit.GetComponent<UnitRenderer>();
                var side = battle.sides.FirstOrDefault(s => s.ID == unit.SideID);
                Color color;
                ColorUtility.TryParseHtmlString(side.Color, out color);
                ur.Become(unit.ID, color);

                units.Add(ur);
            }

            FSM.SetTrigger("GotBattle");
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
