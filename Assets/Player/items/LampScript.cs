using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LampScript : MonoBehaviour
{
    [SerializeField] GameObject cameraP;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(cameraP != null)
        {
            float angle = cameraP.transform.rotation.eulerAngles.x;
            if (angle > 180)
                angle -= 360;
            float normalizedValue = Mathf.InverseLerp(-85f, 85f, angle);
            animator.SetFloat("Blend", normalizedValue);
        }
    }
}
