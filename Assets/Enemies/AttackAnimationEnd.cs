using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationEnd : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemieScript enemieScript = animator.GetComponentInParent<EnemieScript>();
        if (enemieScript != null)
        {
            enemieScript.AttackEnd();
        }
    }
}
