using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotAnimationEnd : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GunScript gunScript = animator.GetComponent<GunScript>();
        if (gunScript != null)
        {
            gunScript.OnShootAnimationEnd();
        }
    }
}
