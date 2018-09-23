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

        public void ShowUnit(Unit unit)
        {
            unitName.text = unit.Name;
        }
    }
}
