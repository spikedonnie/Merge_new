using System;
using UnityEngine;

namespace Assets.FantasyMonsters.Scripts
{
    /// <summary>
    /// This animation script prevents all possible transitions to another states.
    /// </summary>
    public class SoloState : StateMachineBehaviour
    {
        public bool Active;
        public Action Continue;
    bool isCanAttack = false;

        private bool _enter;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("Action", true);
            Active = true;
                    isCanAttack = true;

        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(stateInfo.normalizedTime >= 0.5f && isCanAttack)
            {
                isCanAttack = false;
                animator.GetComponent<Unit>().TakeDamage();
            }

            if (stateInfo.normalizedTime >= 1&& isCanAttack)
            {

                OnStateExit(animator, stateInfo, layerIndex);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Active) return;

            Active = false;

            if (Continue == null)
            {
                animator.SetBool("Action", false);
            }
            else
            {
                Continue();
                Continue = null;
            }
        }
    }
}