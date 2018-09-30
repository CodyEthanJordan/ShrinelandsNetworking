﻿using Assets.Scripts.Controllers;
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

        public void ShowOptions(Battle battle, Dictionary<Guid, string> sides)
        {
            foreach (Transform child in sidesList.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var kvp in sides)
            {
                var newToggle = Instantiate(SideTogglePrefab, sidesList.transform);
                var text = newToggle.transform.GetChild(1).GetComponent<Text>();
                var sideName = battle.sides.First(s => s.ID == kvp.Key).Name;
                text.text = sideName;
                if(kvp.Value != null)
                {
                    text.text = text.text + " : " + kvp.Value;
                }
            }

        }

        public void PlayGame()
        {

        }
    }
}
