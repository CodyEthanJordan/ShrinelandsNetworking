using Assets.Scripts.DungeonMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.View
{
    public class TargetInfo : MonoBehaviour
    {
        public Vector3Int Direction = Vector3Int.zero;

        [SerializeField] private Text InfoText;

        private static readonly List<string> DirectionLabels = new List<string>() { "W", "D", "S", "A" };

        private void Update()
        {
            var rot = Camera.main.transform.rotation.eulerAngles.y;
            float roundedRotation = (float)Math.Round(rot / 90f) * 90f;
            InfoText.rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -roundedRotation));

            if (Direction != Vector3.zero)
            {
                //we've been assigned a direction
                int i = Map.CardinalDirections.IndexOf(Direction);
                i = i - (int)Math.Round(roundedRotation / 90);
                i = (i + 4) % DirectionLabels.Count;
                InfoText.text = DirectionLabels[i];
            }
        }

        public static Vector3Int DirectionFromLocalDirection(Vector3Int localDir)
        {
            var rot = Camera.main.transform.rotation.eulerAngles.y;
            float roundedRotation = (float)Math.Round(rot / 90f) * 90f;
            int i = Map.CardinalDirections.IndexOf(localDir);
            i = i - (int)Math.Round(roundedRotation / 90);
            i = (i + 4) % DirectionLabels.Count;
            return Map.CardinalDirections[i];
        }
    }
}
