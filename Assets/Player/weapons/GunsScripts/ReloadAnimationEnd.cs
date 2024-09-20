using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAnimationEnd : StateMachineBehaviour
{

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ShotgunScript shotgunScript = animator.GetComponent<ShotgunScript>();
        if (shotgunScript != null)
        {
            shotgunScript.OnReloadAnimationEnd();
        }
    }
}
