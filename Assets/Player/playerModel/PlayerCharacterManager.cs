using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlayerCharacterManager : MonoBehaviour
{
    [SerializeField] Transform cameraT;
    Transform headT;

    PlayerMovment playerMovment;
    Transform hipBoneT;
    List<Collider> ragdollColiders = new List<Collider>();
    Animator animator;
    float dotUp;
    Vector3 hipUpDirection;

    // Start is called before the first frame update
    void Start()
    {
        playerMovment = transform.parent.GetComponent<PlayerMovment>();
        SetRagdollColiders();
        animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        hipBoneT = animator.GetBoneTransform(HumanBodyBones.Hips);
        headT = animator.GetBoneTransform(HumanBodyBones.Head);
    }
    public void UpdateHipBone()
    {
        hipUpDirection = hipBoneT.up;
        Vector3 forward = hipBoneT.forward;
        dotUp = Vector3.Dot(forward, Vector3.up);
    }
    private void SetRagdollColiders() 
    {
        Collider[] coliders = this.GetComponentsInChildren<Collider>();
        foreach (Collider colider in coliders)
        {
            colider.isTrigger = true;
            ragdollColiders.Add(colider);
            if (colider.attachedRigidbody != null)
            {
                colider.attachedRigidbody.useGravity = false;
                colider.attachedRigidbody.isKinematic = true;
            }
        }
    }

    public void RagdollStart()
    {
        animator.enabled = false;
        foreach(Collider colider in ragdollColiders)
        {
            colider.isTrigger = false;
            if(colider.attachedRigidbody != null)
            {
                colider.attachedRigidbody.isKinematic = false;
                colider.attachedRigidbody.velocity = playerMovment.playerRigidbody.velocity;
                colider.attachedRigidbody.useGravity = true;
            }
                
        }  
            
    }

    public void RagdollEnd()
    {
        animator.enabled = true;
        animator.Rebind();
        animator.Update(0f);
        foreach (Collider colider in ragdollColiders)
        {
            colider.isTrigger = true;
            if (colider.attachedRigidbody != null)
            {
                colider.attachedRigidbody.useGravity = false;
                colider.attachedRigidbody.isKinematic = true;
            }
        }
            
    }

    public Vector3 HipReycastDown()
    {
        RaycastHit hit;
        if (Physics.Raycast(hipBoneT.position, Vector3.down, out hit, 2))
        {
            return hit.point;
        }
        return hipBoneT.position;
    }
    public Vector3 HipRotationUP()
    {
        Quaternion rotation = Quaternion.LookRotation(hipUpDirection); // zamiana wektora na kwaternion
        return rotation.eulerAngles; // zamiana kwaternionu na Eulery
    }
    public Vector3 HipRotationDown()
    {
        Quaternion rotation = Quaternion.LookRotation(-hipUpDirection); // zamiana wektora na kwaternion
        return rotation.eulerAngles; // zamiana kwaternionu na Eulery
    }
    public bool HipFacingUp()
    {
        if (dotUp > 0f)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    public void CameraParentHead()
    {
        cameraT.parent = headT;
    }

    public void CameraParentReset()
    {
        cameraT.parent = transform.parent;
    }
}
