using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class UnitRenderer : MonoBehaviour
    {
        public Guid UnitRepresented;
        public Material BaseMaterial;

        private MeshRenderer mr;

        private void Awake()
        {
            mr = GetComponent<MeshRenderer>();
        }

        public void Become(Guid unitRepresented, Color color)
        {
            this.UnitRepresented = unitRepresented;

            Material mat = new Material(BaseMaterial);
            mat.color = color;

            mr.material = mat;
        }
    }
}
