using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEEEEEEST : MonoBehaviour
{
    public Transform inne;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hipUpDirection = transform.up;
        inne.rotation = Quaternion.LookRotation(hipUpDirection); // zamiana wektora na kwaternion
    }
}
