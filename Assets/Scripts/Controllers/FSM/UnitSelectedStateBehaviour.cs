using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Controllers.FSM
{
    public class UnitSelectedStateBehaviour : GameStateBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            bc.ShowUnitStats(bc.SelectedUnit.UnitRepresented);
            bc.RenderMovementOptions(bc.SelectedUnit.UnitRepresented);
            bc.OnTargetClick += MoveUnit;
        }

        private void MoveUnit(object source, Vector3 position)
        {
            Vector3Int target = new Vector3Int((int)position.x, (int)position.y, (int)position.z);
            bc.client.MoveUnit(bc.SelectedUnit.UnitRepresented, target);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            bc.RemoveTargets();
            bc.HideUnitStats();
            bc.OnTargetClick -= MoveUnit;
        }
      
    }
}
