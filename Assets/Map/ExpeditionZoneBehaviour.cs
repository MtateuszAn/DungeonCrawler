using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionZoneBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SaveSystemBehaviour.Instance.LoadNewExpedition();
        }
    }
}
