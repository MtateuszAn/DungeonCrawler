using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class PlayerItemPicup : MonoBehaviour
{
    [SerializeField] GameObject jointGO;
    [Header("JointOptions")]
    [SerializeField] float linearLimit;
    [SerializeField] float linearLimitSpring;
    [SerializeField] float linearLimitSpringDamper;
    [SerializeField] float springForce;
    [SerializeField] float springDamper;
    [SerializeField] float breakForce;
    [SerializeField] float breakTorque;
    ConfigurableJoint joint;
    PlayerInteractBehaviour takeToInventorySc;

    PlayerInput playerInput;
    InputAction picUpAction;

    Rigidbody courentItemRB;


    private void Start()
    {
        if (!jointGO.TryGetComponent<ConfigurableJoint>(out joint)) 
        {
            createJoint();
        }
        takeToInventorySc = GetComponent<PlayerInteractBehaviour>();
        playerInput = GetComponent<PlayerInput>();
        picUpAction = playerInput.actions["PicUp"];
        picUpAction.started += ctx => PicUpRB();
        picUpAction.canceled += ctx => DropRB();
    }

    void createJoint()
    {
        joint = jointGO.AddComponent<ConfigurableJoint>();
        //joint.autoConfigureConnectedAnchor = false;
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = joint.linearLimit;
        limit.limit = linearLimit;
        joint.linearLimit = limit;

        SoftJointLimitSpring limitS = joint.linearLimitSpring;
        limitS.spring = linearLimitSpring;
        limitS.damper = linearLimitSpringDamper;
        joint.linearLimitSpring = limitS;

        JointDrive jointDrive = joint.xDrive;
        jointDrive.positionSpring = springForce;
        jointDrive.positionDamper = springDamper;
        joint.xDrive= jointDrive;
        joint.yDrive= jointDrive;
        joint.zDrive= jointDrive;

        joint.rotationDriveMode = RotationDriveMode.Slerp;

        joint.slerpDrive = jointDrive;

        joint.breakForce = breakForce;
        joint.breakTorque = breakTorque;
    }

    private void PicUpRB()
    {
        //Debug.Log("RMB Presed");
        if (takeToInventorySc.itemDetected != null)
        {
            if (joint == null)
            {
                createJoint();
            }
            jointGO.transform.position = takeToInventorySc.hit.point;
            //Debug.Log("Item PICTUP");
            courentItemRB = takeToInventorySc.itemDetected.gameObject.GetComponent<Rigidbody>();
            joint.connectedBody = courentItemRB;
        }
    }
    private void DropRB()
    {
        //Debug.Log("RMB relised");
        if(courentItemRB != null && joint != null)
        {
            courentItemRB.WakeUp();
            joint.connectedBody = null;
        }
        courentItemRB = null;
    }

}

