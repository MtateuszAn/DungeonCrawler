using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class PlayerMovment : MonoBehaviour
{
    public static PlayerMovment Instance;

    [SerializeField] float movementSpeed = 9f; //Spead of player walking
    [SerializeField] float fallingSpeed = 9f; //Spead of player falling
    [SerializeField] float sprintSpeedModyfier = 2f; //Aditional speed when sprinting
    [SerializeField] float drag = 6f; // Mas of player for gravitation
    [SerializeField] float movingFactorWhileFalling = 0.2f; // Mas of player for gravitation
    //[SerializeField] float acceleration = 20f; //Value for smooth start and stop when moving
    [SerializeField] float jumpForce = 5f; //Value for force of player jump
    private float moveModyfier = 1;

    public Rigidbody playerRigidbody;
    PlayerInput playerInput;
    public InputAction moveAction;
    InputAction sprintAction;
    public InputAction jumpAction;
    private Action<InputAction.CallbackContext> jumpActionDelegate;

    [SerializeField] public Animator animator;

    [SerializeField] internal Vector3 velocity;
    bool grounded = false;
    bool wasGrounded = false;
    
    void Start()
    {
        Instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        playerRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        //animator = GetComponent<Animator>();
        moveAction = playerInput.actions["PlayerMovment"];
        sprintAction = playerInput.actions["PlayerSprint"];
        jumpAction = playerInput.actions["PlayerJump"];
        jumpActionDelegate = ctx => PlayerJump();
        jumpAction.performed += jumpActionDelegate;
    }
    private void OnDisable()
    {
        playerRigidbody.drag = drag;
    }
    private void OnDestroy()
    {
        jumpAction.performed -= jumpActionDelegate;
    }
    public void UpdateGravity()
    {
        grounded = Physics.Raycast(transform.position-new Vector3(0,0.9f,0),Vector3.down,0.2f);
        if(grounded!=wasGrounded)
        {
            GroundedStateChange();
            wasGrounded = grounded;
        }

        if (!grounded)
        {
            playerRigidbody.drag = 0.1f;
            playerRigidbody.AddForce(Vector3.down * fallingSpeed, ForceMode.Acceleration);
            //Debug.Log(playerRigidbody.velocity.y);
            if(playerRigidbody.velocity.y < -10 && !PlayerStateManager.Instance.isRagdoll)
            {
                PlayerStateManager.Instance.StartRagdollAction.Invoke();
            }
        }
        else
        {
            playerRigidbody.drag = drag;
        }
    }

    private void GroundedStateChange()
    {
        //just Landed
        if (grounded)
        {
            //Debug.Log("LANDED : " + playerRigidbody.velocity.z);
        }
    }

    public void UpdateWalk()
    {
        //Taking variables from ceybord
        moveModyfier = 1;
        moveModyfier += sprintAction.ReadValue<float>()*sprintSpeedModyfier;
        var moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput != Vector2.zero && grounded)
        {
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }

        var moveDirection = new Vector3();
        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        //falling slows mowment axeleration
        if (!grounded)
        {
            moveModyfier *= movingFactorWhileFalling;
        }

        playerRigidbody.AddForce(moveDirection.normalized * movementSpeed * moveModyfier,ForceMode.Acceleration);

    }
    void PlayerJump()
    {
        if (grounded && !playerRigidbody.isKinematic)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    internal void GroundState()
    {
        throw new NotImplementedException();
    }
}
