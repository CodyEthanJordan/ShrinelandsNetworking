using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class BlockRenderer : MonoBehaviour
    {
        private MeshRenderer mr;
        private Collider collider;

        private void Awake()
        {
            mr = GetComponent<MeshRenderer>();
            collider = GetComponent<Collider>();
        }

        public string BlockRepresented;

        public void Become(string blockType)
        {
            this.BlockRepresented = blockType;
            mr.enabled = true;
            collider.enabled = true;
            //TODO: materials for other blocks
            switch(blockType)
            {
                case "air":
                    mr.enabled = false;
                    collider.enabled = false;
                    break;
            }
        }
    }
}
