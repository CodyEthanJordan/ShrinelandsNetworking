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
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            bc.HideUnitStats();
        }
      
    }
}
