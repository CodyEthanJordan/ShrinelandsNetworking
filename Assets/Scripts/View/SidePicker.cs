using Assets.Scripts.Controllers;
using Assets.Scripts.DungeonMaster;
using Assets.Scripts.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.View
{
    public class SidePicker : MonoBehaviour
    {
        public BattleController bc;
        public GameObject sidesList;
        public GameObject SideTogglePrefab;

        public void ShowOptions(Dictionary<Side, PlayerInfo> sides)
        {
            foreach (Transform child in sidesList.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var side in sides)
            {
                var newToggle = Instantiate(SideTogglePrefab, sidesList.transform);
                var text = newToggle.transform.GetChild(1).GetComponent<Text>();
                text.text = side.Key.Name;
                if(side.Value != null)
                {
                    text.text = text.text + " : " + side.Value.Name;
                }
            }

        }

        public void PlayGame()
        {

        }
    }
}
