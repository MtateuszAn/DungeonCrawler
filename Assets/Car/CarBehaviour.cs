using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarBehaviour : MonoBehaviour
{
    public static CarBehaviour Instance;

    [SerializeField] float accelerationForce;
    [SerializeField] float breakingForce;
    [SerializeField] float maxTurnAngle =15f;
    float courentTurnAngle;

    [SerializeField] public Transform smallItemsParent;
    [SerializeField] public Transform largeItemsParent;
    [SerializeField] public Transform largeContainerParent;

    [SerializeField] public Transform sittingParent;
    [SerializeField] public CarSeatBehaviour mainSeat;
    [SerializeField] public List<Transform> getOutOfCarTransform;

    [SerializeField] Transform sterengWheel;
    [SerializeField] WheelCollider wheelFR;
    [SerializeField] WheelCollider wheelFL;
    [SerializeField] WheelCollider wheelRR;
    [SerializeField] WheelCollider wheelRL;

    [SerializeField] Transform wheelFRMesh;
    [SerializeField] Transform wheelFLMesh;
    [SerializeField] Transform wheelRRMesh;
    [SerializeField] Transform wheelRLMesh;

    public bool inCar=false;

    InputAction moveAction;
    InputAction breakAction;

    private void Start()
    {
        Instance = this;

        foreach (Transform t in getOutOfCarTransform)
            t.gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (inCar)
        {
            float breaking = breakAction.ReadValue<float>();
            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            float acceleration = moveInput.y * accelerationForce;
            courentTurnAngle = Mathf.Lerp(courentTurnAngle, moveInput.x * maxTurnAngle, 0.05f);
            CarAcceleration(acceleration);
            CarBreaking(breaking);
            CarStearing();

            
            PlayerStateManager.Instance.transform.position = sittingParent.transform.position;
            PlayerStateManager.Instance.transform.rotation = sittingParent.transform.rotation;
        }
        UpdateWheel(wheelFR, wheelFRMesh);
        UpdateWheel(wheelFL, wheelFLMesh);
        UpdateWheel(wheelRR, wheelRRMesh);
        UpdateWheel(wheelRL, wheelRLMesh);
    }
    void CarAcceleration(float acceleration)
    {
        wheelRR.motorTorque = acceleration;
        wheelRL.motorTorque = acceleration;
    }
    void CarBreaking(float breaking)
    {
        wheelFR.brakeTorque = breaking * breakingForce;
        wheelFL.brakeTorque = breaking * breakingForce;
        wheelRR.brakeTorque = breaking * breakingForce;
        wheelRL.brakeTorque = breaking * breakingForce;
    }
    void CarStearing()
    {
        wheelFR.steerAngle = courentTurnAngle;
        wheelFL.steerAngle = courentTurnAngle;

        sterengWheel.localEulerAngles = new Vector3(0f, courentTurnAngle * 2, 0f);
    }
    void UpdateWheel(WheelCollider colider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;
        colider.GetWorldPose(out position, out rotation);

        transform.position = position;
        transform.rotation = rotation;
    }
    public void GetInCar()
    {
        Debug.Log("PlayerGotInCar - CarBech");
        if (moveAction == null)
        {
            moveAction = PlayerMovment.Instance.moveAction;
            breakAction = PlayerMovment.Instance.jumpAction;
        }
        inCar = true;
        
        foreach (Transform t in getOutOfCarTransform)
            t.gameObject.SetActive(true);
    }
    public void GetOutOfCar()
    {
        inCar = false;
        foreach (Transform t in getOutOfCarTransform)
            t.gameObject.SetActive(false);
    }
    
}
