using UnityEngine;


public class InventoryBeltSlotBehaviour : InventoryBehaviour
{
    void Update()
    {
        if ((Screen.width != InventoryBehaviour.lastScreenSize.x || Screen.height != InventoryBehaviour.lastScreenSize.y) && inventoryUI != null)
        {
            inventoryUI.enabled = false;
            inventoryUI.enabled = true;
        }
    }

}
