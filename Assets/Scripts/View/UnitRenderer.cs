using Assets.Scripts.DungeonMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class UnitRenderer : MonoBehaviour
    {
        public Unit UnitRepresented;
        public Material BaseMaterial;

        private MeshRenderer mr;

        private void Awake()
        {
            mr = GetComponent<MeshRenderer>();
        }

        public void Become(Unit unitRepresented, Color color)
        {
            this.UnitRepresented = unitRepresented;

            Material mat = new Material(BaseMaterial);
            mat.color = color;

            mr.material = mat;

            UnitRepresented.OnUnitMoved += HandleMoveEvent;
        }

        private void HandleMoveEvent(object source, Guid ID, Vector3Int oldPos, Vector3Int newPos)
        {
            this.transform.position = new Vector3(newPos.x, newPos.z, newPos.y);
        }

        private void OnDestroy()
        {
            UnitRepresented.OnUnitMoved -= HandleMoveEvent;
        }
    }
}
