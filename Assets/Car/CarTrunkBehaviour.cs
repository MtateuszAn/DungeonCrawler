using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrunkBehaviour : MonoBehaviour
{
    [SerializeField] public Transform smallItemsParent;
    [SerializeField] public Transform largeItemsParent;
    [SerializeField] public Transform largeContainerParent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==11)
        {
            if (other.GetComponent<SmalItemBehaviour>() != null)
            {
                other.transform.parent = smallItemsParent;
            }
            if (other.GetComponent<LargeItemBehaviour>() != null)
            {
                if (other.GetComponent<InventoryConteinerBehaviour>() != null)
                {
                    other.transform.parent = largeContainerParent;
                }
                else
                {
                    other.transform.parent = largeItemsParent;
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
        if (ItemsParentsControllerBehaviour.Instance != null)
        { 
            if (other.GetComponent<SmalItemBehaviour>() != null)
            {
                other.transform.parent = ItemsParentsControllerBehaviour.Instance.smallItemsParent;
            }
            else if (other.GetComponent<LargeItemBehaviour>() != null)
            {
                if (other.GetComponent<InventoryConteinerBehaviour>() != null)
                {
                    other.transform.parent = ItemsParentsControllerBehaviour.Instance.largeContainerParent;
                }
                else
                {
                    other.transform.parent = ItemsParentsControllerBehaviour.Instance.largeItemsParent;
                }
            }
        }
        else
        {
            other.transform.parent = null;
        }
        }
    }
}
