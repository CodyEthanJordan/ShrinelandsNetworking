using Assets.Scripts.DungeonMaster;
using Assets.Scripts.Networking;
using Assets.Scripts.View;
using Newtonsoft.Json;
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
        public GameObject TargetCubePrefab;
        public Camera Cam;

        public float Speed;
        public SmoothMouseLook mouseLook;

        public Transform UnitParent;
        public Transform BlockParent;

        public UnitRenderer SelectedUnit;
        public UnitInfoPanel unitInfoPanel;

        [SerializeField] private Text serverIPText;
        [SerializeField] private InputField playerNameInput;
        [SerializeField] private GameObject connectionPanel;
        [SerializeField] private GameObject chooseSidePanel;
        public Client client;

        private List<BlockRenderer> blocks = new List<BlockRenderer>();
        private List<UnitRenderer> units = new List<UnitRenderer>();
        private List<TargetInfo> targetCubes = new List<TargetInfo>();
        public Animator FSM;

        public event TargetClickedEvent OnTargetClick;
        public event DirectionChosenEvent OnChooseDirection;

        public Side playingAsSide;

        private void Awake()
        {
            FSM = GetComponent<Animator>();
        }

        private void Start()
        {
            client.OnConnected += Connected;
            client.OnRecieveBattle += InitializeBattle;
            client.OnRecieveSides += PickSides;

            mouseLook.enabled = false;
            unitInfoPanel.gameObject.SetActive(false);

            LoadPlayerPrefs();
        }

        private void PickSides(object source, Dictionary<Guid, string> sides)
        {
            chooseSidePanel.SetActive(true);
            chooseSidePanel.GetComponent<SidePicker>().ShowOptions(client.battle, sides);
        }

        private void LoadPlayerPrefs()
        {
            if(PlayerPrefs.HasKey("PlayerName"))
            {
                playerNameInput.text = PlayerPrefs.GetString("PlayerName");
            }
        }

        private void Update()
        {
            HandleCameraMovement();
            HandleClickingStuff();

            if(!Input.GetMouseButton(1))
            {
                //not doing camera movement, can do keyboard shortcuts

                if(Input.GetKeyUp(KeyCode.W))
                {
                    if(OnChooseDirection != null)
                    {
                        var dir = TargetInfo.DirectionFromLocalDirection(Vector3Int.up);
                        OnChooseDirection(this, dir);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.D))
                {
                    if (OnChooseDirection != null)
                    {
                        var dir = TargetInfo.DirectionFromLocalDirection(Vector3Int.right);
                        OnChooseDirection(this, dir);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.S))
                {
                    if (OnChooseDirection != null)
                    {
                        var dir = TargetInfo.DirectionFromLocalDirection(Vector3Int.down);
                        OnChooseDirection(this, dir);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.A))
                {
                    if (OnChooseDirection != null)
                    {
                        var dir = TargetInfo.DirectionFromLocalDirection(Vector3Int.left);
                        OnChooseDirection(this, dir);
                    }
                }
            }
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
                    else if(hit.transform.gameObject.CompareTag("Target"))
                    {
                        if(OnTargetClick != null)
                        {
                            OnTargetClick(this, hit.transform.position);
                        }
                    }
                    else
                    {
                        FSM.SetTrigger("Deselect"); //TODO: let FSM handle this stuff
                    }
                }
            }
        }

        public void EndTurn()
        {
            NetworkMessage message = new NetworkMessage("end turn", 
                JsonConvert.SerializeObject(playingAsSide.ID));
            client.SendToServer(message);
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

        public void RenderMovementOptions(Guid unitRepresented)
        {
            RemoveTargets();
            foreach (var kvp in client.battle.GetValidMovements(unitRepresented))
            {
                var newCube = Instantiate(TargetCubePrefab, this.transform, true);
                newCube.transform.position = new Vector3(kvp.Value.x, kvp.Value.z, kvp.Value.y);
                var targetInfo = newCube.GetComponent<TargetInfo>();
                targetInfo.Direction = kvp.Key;
                targetCubes.Add(targetInfo);
            }
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

            if(Input.GetMouseButton(1))
            {
                Cam.transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * Speed * Time.deltaTime, Space.Self);
                Cam.transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * Speed * Time.deltaTime, Space.Self);
                Cam.transform.Translate(Vector3.up * Input.GetAxis("Jump") * Speed * Time.deltaTime, Space.World);
            }
           

        }

        public void RemoveTargets()
        {
            foreach (var cube in targetCubes)
            {
                Destroy(cube.gameObject);
            }
            targetCubes.Clear();
        }

        private void CleanUp()
        {
            RemoveTargets();
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
                ur.Become(unit, color);

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
            PlayerPrefs.SetString("PlayerName", playerNameInput.text);
            PlayerPrefs.Save();
            client.ConnectToServer(serverIPText.text, playerNameInput.text);
        }
    }

    public delegate void TargetClickedEvent(object source, Vector3 position);
    public delegate void DirectionChosenEvent(object source, Vector3Int position);
    public delegate void SelectedUnitMovedEvent(object source, Vector3Int position);
    public delegate void SelectedUnitStatChangedEvent(object source, Unit unit);
}
