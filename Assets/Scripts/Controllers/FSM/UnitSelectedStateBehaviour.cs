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

            bc.ShowUnitStats(bc.SelectedUnit.UnitRepresented.ID);
            bc.RenderMovementOptions(bc.SelectedUnit.UnitRepresented.ID);
            bc.OnTargetClick += MoveUnit;
            bc.OnChooseDirection += MoveUnitDirection;
        }

        private void MoveUnitDirection(object source, Vector3Int dir)
        {
            Vector3Int target = bc.SelectedUnit.UnitRepresented.Position + dir;
            bc.client.MoveUnit(bc.SelectedUnit.UnitRepresented.ID, target);
            bc.RenderMovementOptions(bc.SelectedUnit.UnitRepresented.ID);
        }

        private void MoveUnit(object source, Vector3 position)
        {
            Vector3Int target = new Vector3Int((int)position.x, (int)position.z, (int)position.y);
            bc.client.MoveUnit(bc.SelectedUnit.UnitRepresented.ID, target);
            bc.RenderMovementOptions(bc.SelectedUnit.UnitRepresented.ID);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            bc.RemoveTargets();
            bc.HideUnitStats();
            bc.OnTargetClick -= MoveUnit;
            bc.OnChooseDirection -= MoveUnitDirection;
        }
      
    }
}
