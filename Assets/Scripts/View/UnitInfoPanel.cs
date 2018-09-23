using Assets.Scripts.DungeonMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.View
{
    public class UnitInfoPanel : MonoBehaviour
    {
        [SerializeField] private Text unitName;
        [SerializeField] private Text hp;
        [SerializeField] private Text move;

        public void ShowUnit(Unit unit)
        {
            unitName.text = unit.Name;
            hp.text = 
        }
    }
}
