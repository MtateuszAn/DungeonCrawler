using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemBehaviour : MonoBehaviour
{
    protected virtual string itemName { get; set; }
    public string GetItemName()
    {
        return itemName;
    }
}
