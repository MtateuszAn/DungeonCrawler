using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject playerGameObject;
    Rigidbody playerRigidbody;
    PlayerStateManager stateManager;
    PlayerInput playerInput;
    InputAction lookAction;//Input Action from Player action Map
    [SerializeField] float mouseSensitivity = 3f; //Sensitivity of player looking around
    public Vector2 look;//direction of player camera
    Vector3 CameraDefoultPosition;
    // Start is called before the first frame update
    void Start()
    {
        stateManager = playerGameObject.GetComponent<PlayerStateManager>();
        playerInput = playerGameObject.GetComponent<PlayerInput>();
        playerRigidbody = playerGameObject.GetComponent<Rigidbody>();
        lookAction = playerInput.actions["PlayerLook"];
        CameraDefoultPosition = transform.localPosition;
    }

    public void UpdateLook()
    {
        //Taking variables from mouse
        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSensitivity;
        look.y += lookInput.y * mouseSensitivity;
        //Restriction on lucking up and down
        look.y = Mathf.Clamp(look.y, -85f, 85f);
        //Roteiting camera and player
        transform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        //playerGameObject.transform.localRotation = Quaternion.Euler(0, look.x, 0);
        // Obracanie graczem za pomoc¹ fizyki (na osi Y - lewo/prawo)
        Quaternion playerRotation = Quaternion.Euler(0, look.x, 0);
        playerRigidbody.MoveRotation(playerRotation);
    }
    public void UpdateLookInCar()
    {
        //Taking variables from mouse
        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSensitivity;
        look.y += lookInput.y * mouseSensitivity;
        //Restriction on lucking up and down
        look.y = Mathf.Clamp(look.y, -85f, 85f);
        //Roteiting camera
        transform.localRotation = Quaternion.Euler(-look.y, look.x, 0);
    }
    public void CameraPositionRestart()
    {
        transform.localPosition = CameraDefoultPosition;
    }
}
