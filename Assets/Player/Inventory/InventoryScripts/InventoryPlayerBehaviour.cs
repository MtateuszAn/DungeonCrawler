using UnityEngine;


public class InventoryPlayerBehaviour : InventoryBehaviour
{
    //Only Player Inventory Updates 
    void Update()
    {
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            tileSize = (int)Mathf.Lerp(0f, 50, Screen.height / 998f);
            if(inventoryUI != null)
            {
                inventoryUI.enabled = false;
                inventoryUI.enabled = true;
            }
            lastScreenSize = new Vector2(Screen.width, Screen.height);
        }
    }
}
