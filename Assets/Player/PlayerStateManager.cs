using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager Instance { get; private set; }

    PlayerMovment movment;
    [SerializeField] private TMP_Text FPSCounter;
    [SerializeField] public CameraManager cameraM;
    [SerializeField] public BeltManager beltManager;
    [SerializeField] PlayerCharacterManager characterManager;

    PlayerInput playerInput;
    Rigidbody playerRigidbody;
    CapsuleCollider playerColider;
    InputAction ragdollAction;
    Transform playerParent;

    public bool canMove = true;
    public bool canLook = true;
    public bool isAlive = true;
    public bool isRagdoll = false;
    public bool inCar = false;
    public bool animationPlaying = false;

    public UnityAction StartRagdollAction;
    public UnityAction EndRagdollAction;

    private System.Action<InputAction.CallbackContext> ragdollStartActionDelegate;
    private System.Action<InputAction.CallbackContext> ragdollEndActionDelegate;

    private void Start()
    {
        Instance = this;
        playerParent = transform.parent;
        playerInput = GetComponent<PlayerInput>();
        ragdollAction = playerInput.actions["Ragdoll"];
        movment = GetComponent<PlayerMovment>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerColider = GetComponent<CapsuleCollider>();
        
        StartRagdollAction += characterManager.RagdollStart;
        StartRagdollAction += RagdollStart;
        ragdollStartActionDelegate = ctx => StartRagdollAction?.Invoke();

        EndRagdollAction += RagdollEnd;
        ragdollEndActionDelegate = ctx => EndRagdollAction?.Invoke();

        ragdollAction.performed += ragdollStartActionDelegate;
        StartCoroutine(UpdateFPS());
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    private IEnumerator UpdateFPS()
    {
        while (true)
        {
            FPSCounter.text = "" + (int)(1 / Time.deltaTime);
            yield return new WaitForSeconds(1);
        }
    }

    private void Update()
    {
        if (!isRagdoll) 
        {
            movment.UpdateGravity();
        }

        if (isAlive && canMove && !isRagdoll && !animationPlaying && !inCar)
            movment.UpdateWalk();

        if (isAlive && canLook && !isRagdoll && !animationPlaying)
        {
            if (!inCar)
            {
                cameraM.UpdateLook();
            }
            else
            {
                cameraM.UpdateLookInCar();
            }
        }
            
    }
    #region Ragdoll
    void RagdollStart()
    {
        StartCoroutine(CheckRagdollEnd());
        characterManager.CameraParentHead();
        ragdollAction.performed -= ragdollStartActionDelegate;
        ragdollAction.performed += ragdollEndActionDelegate;
        if(!playerRigidbody.isKinematic)
            playerRigidbody.velocity= Vector3.zero;
        playerRigidbody.isKinematic = true;
        playerColider.isTrigger = true;
        isRagdoll = true;
    }
    Vector3 lastRagdollPosition = Vector3.zero;
    private IEnumerator CheckRagdollEnd() 
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            characterManager.UpdateHipBone();
            Vector3 courentRagdollPosition = characterManager.HipReycastDown();
            Vector3 roundedRagdollPosition = new Vector3(
                Mathf.Round(courentRagdollPosition.x * 1000f) / 1000f,
                Mathf.Round(courentRagdollPosition.y * 1000f) / 1000f,
                Mathf.Round(courentRagdollPosition.z * 1000f) / 1000f
            );
            if (lastRagdollPosition == roundedRagdollPosition)
            {
                RagdollEnd();
            }
            else
            {
                lastRagdollPosition = roundedRagdollPosition;
            }
        }
    }
    private void RagdollEnd()
    {
        StopAllCoroutines();
        if (inCar)
        {
            RagdollEndIncar();
            return;
        }
        PlayerPositionToHips();
        characterManager.RagdollEnd();

        movment.velocity = Vector3.zero;
        movment.animator.SetBool("walking", false);
        if (characterManager.HipFacingUp())
        {
            PlayerRotationToHips(characterManager.HipRotationDown().y);
            movment.animator.Play("standUpBack");
        }
        else
        {
            PlayerRotationToHips(characterManager.HipRotationUP().y);
            movment.animator.Play("standUpFront");
        }

        //movment.animator.SetTrigger("getUp");
        
        ragdollAction.performed += ragdollStartActionDelegate;
        ragdollAction.performed -= ragdollEndActionDelegate;
        
        isRagdoll = false;
        animationPlaying=true;
    }
    private void RagdollEndIncar()
    {
        characterManager.RagdollEnd();
        movment.animator.Play("sitInCar");
        transform.localEulerAngles = Vector3.zero;
        playerRigidbody.position = Vector3.zero;
        ragdollAction.performed += ragdollStartActionDelegate;
        ragdollAction.performed -= ragdollEndActionDelegate;
        
        isRagdoll = false;
    }
    private void PlayerRotationToHips(float rotY)
    {
        cameraM.look.x = rotY;
        Quaternion playerRotation = Quaternion.Euler(0, rotY, 0);
        playerRigidbody.MoveRotation(playerRotation);
    }

    private void PlayerPositionToHips()
    {
        Vector3 reycastResult = characterManager.HipReycastDown() + new Vector3(0f, 1f, 0f);
        playerRigidbody.position = reycastResult;
    }
    public void GetUpAnimationEnd()
    {
        characterManager.CameraParentReset();
        cameraM.CameraPositionRestart();
        playerColider.isTrigger = false;
        playerRigidbody.isKinematic = false;
        animationPlaying =false;
    }
    #endregion
    #region Car
    CarSeatBehaviour courentSeatBehaviour;
    public void GetInCar(CarSeatBehaviour seatBehaviour)
    {

        if (courentSeatBehaviour != null)
        {
            courentSeatBehaviour.gameObject.SetActive(true);
        }

        CarBehaviour.Instance.GetInCar();

        courentSeatBehaviour = seatBehaviour;
        seatBehaviour.gameObject.SetActive(false);
        inCar = true;

        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.isKinematic=true;
        playerColider.isTrigger = true;
        movment.animator.Play("sitInCar");
        
        characterManager.CameraParentHead();
        transform.parent = CarBehaviour.Instance.sittingParent;
        cameraM.look = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.localPosition = Vector3.zero;
        
    }
    public void GetOutOfCar(GetOutBehaviour getOut)
    {
        courentSeatBehaviour.gameObject.SetActive(true);
        courentSeatBehaviour = null;

        CarBehaviour.Instance.GetOutOfCar();
        
        inCar = false;
        Vector3 reycastResult = getOut.ReycastDown() + new Vector3(0f, 1f, 0f);
        transform.position = reycastResult;
        // Resetowanie wszystkich parametrów animatora do wartoœci pocz¹tkowych
        movment.animator.Rebind();

        // Ustawienie animatora na pierwsz¹ animacjê w bie¿¹cym stanie
        movment.animator.Play(movment.animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);

        transform.parent = playerParent;
        playerColider.isTrigger=false;
        playerRigidbody.isKinematic = false;
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.position = reycastResult;

        characterManager.CameraParentReset();
        cameraM.CameraPositionRestart();
    }
    #endregion
}
