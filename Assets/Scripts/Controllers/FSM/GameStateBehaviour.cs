using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts.Controllers.FSM
{
    public class GameStateBehaviour : StateMachineBehaviour
    {
        protected BattleController bc;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            bc = animator.gameObject.GetComponent<BattleController>();
        }
    }
}
