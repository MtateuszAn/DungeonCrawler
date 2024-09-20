using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    InputAction lookAction;

    void Start()
    {
        lookAction = playerInput.actions["PlayerLook"];
    }


    private float swayAmountX = 0.06f;
    private float swayAmountY = 0.03f;
    private float smooth = 12;

    private void Update()
    {
        var lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * swayAmountX;
        float mouseY = lookInput.y * swayAmountY;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRot = rotationX * rotationY;

        Quaternion interpolatedRot = Quaternion.Slerp(transform.localRotation, targetRot, smooth * Time.deltaTime);

        // Konwersja do k¹tów Eulera
        Vector3 eulerRotation = interpolatedRot.eulerAngles;

        // Korekta zakresu k¹tów Eulera od 0-360 do -180-180
        if (eulerRotation.x > 180) eulerRotation.x -= 360;
        if (eulerRotation.y > 180) eulerRotation.y -= 360;
        if (eulerRotation.z > 180) eulerRotation.z -= 360;

        // Ograniczenie k¹tów Eulera do zakresu od -45 do 45 stopni
        eulerRotation.x = Mathf.Clamp(eulerRotation.x, -45f, 45f);
        eulerRotation.y = Mathf.Clamp(eulerRotation.y, -45f, 45f);
        eulerRotation.z = Mathf.Clamp(eulerRotation.z, -45f, 45f);

        // Konwersja z powrotem do kwaternionów
        transform.localRotation = Quaternion.Euler(eulerRotation);
    }
}
