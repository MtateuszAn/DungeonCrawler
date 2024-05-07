using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovment : MonoBehaviour
{
    CharacterController controller;
    PlayerInput playerInput;

    public Transform cameraTransform; // Camera pozition And rotation
    [SerializeField] float mouseSensitivity = 3f; //Sensitivity of player looking around
    [SerializeField] float movementSpeed = 9f; //Spead of player walking
    [SerializeField] float mass = 2f; // Mas of player for gravitation
    [SerializeField] float acceleration = 20f; //Value for smooth start and stop when moving
    [SerializeField] Image healthBar;
    [SerializeField] float maxHealth = 100;
    float health;

    InputAction lookAction;//Input Action from Player action Map

    InputAction moveAction;

    internal Vector3 velocity;
    Vector2 look;//direction of player camera
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["PlayerMovment"];
        lookAction = playerInput.actions["PlayerLook"];
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWalk();
        UpdateLook();
    }
    void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }
    
    void UpdateWalk()
    {
        //Taking variables from ceybord
        var moveInput = moveAction.ReadValue<Vector2>();
        var input = new Vector3();
        input += transform.forward * moveInput.y;
        input += transform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f);
        input *= movementSpeed;
        //Making movment smooth
        var factor = acceleration * Time.deltaTime;
        //falling slows mowment axeleration
        if (!controller.isGrounded)
        {
            factor = factor / 15;
        }

        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);


        controller.Move(velocity * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        health-= damage;
        healthBar.fillAmount = health/maxHealth;
    }
    void UpdateLook()
    {
        //Taking variables from mouse
        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSensitivity;
        look.y += lookInput.y * mouseSensitivity;
        //Restriction on lucking up and down
        look.y = Mathf.Clamp(look.y, -85f, 85f);
        //Roteiting camera and player
        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }
}
