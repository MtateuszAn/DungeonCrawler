using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual string GetText()
    {
        return "NULL";
    }
    public virtual bool Interact()
    {
        Debug.Log("Interacted Not Implemented");
        return true;
    }
}
