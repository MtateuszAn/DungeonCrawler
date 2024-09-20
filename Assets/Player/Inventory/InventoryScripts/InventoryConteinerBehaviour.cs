using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InventoryConteinerBehaviour : InventoryBehaviour
{
    void Update()
    {
        if ((Screen.width != InventoryBehaviour.lastScreenSize.x || Screen.height != InventoryBehaviour.lastScreenSize.y)&& inventoryUI != null)
        {
            inventoryUI.enabled = false;
            inventoryUI.enabled = true;
        }
    }
    public void OnInvUiUnlink()
    {
        Animator animator;
        if (gameObject.TryGetComponent<Animator>(out animator))
        {
            animator.SetBool("IsOpen",false);
        }
    }
    public void OnInvUiLink()
    {
        Animator animator;
        if (gameObject.TryGetComponent<Animator>(out animator))
        {
            animator.SetBool("IsOpen", true);
        }
    }
}
