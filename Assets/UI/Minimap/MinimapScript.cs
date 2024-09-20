using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour
{

    [SerializeField] Transform player;
    void LateUpdate()
    {
        transform.position = player.position;
    }
}
