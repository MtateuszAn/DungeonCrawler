using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoHomeZoneBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SaveSystemBehaviour.Instance.LoadHomeFromExpedition();
        }
    }
}
