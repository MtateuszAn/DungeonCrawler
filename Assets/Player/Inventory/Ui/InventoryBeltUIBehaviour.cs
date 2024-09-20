using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]

public class InventoryBeltUIBehaviour : InventoryUIBehaviour
{
    // Handles the dragging of an item across the inventory for Belt slot type inventory
    override internal List<UiInventorySlot> ItemDrag(InventoryItem itemDragged)
    {
        ClearColor();  // Clear slot highlights
        //Only 1 item per slot and ony items cass ItemHold or child cass
        if (inventory.items.Count > 1 || itemDragged.item is not ItemHold)
        {
            Debug.Log("Nie jest Hold");
            return null;
        }
            

        List<Vector2Int> hoveredSlotsPozitions = new List<Vector2Int>();

        for (int j = 0; j < itemDragged.height; j++)
        {
            for (int i = 0; i < itemDragged.width; i++)
            {
                hoveredSlotsPozitions.Add(new Vector2Int(i, j));

                if (i >= inventory.invWidth || j >= inventory.invHeight || i < 0 || j < 0)
                    return null;
            }
        }

        List<UiInventorySlot> hoveredSlots = new List<UiInventorySlot>();

        foreach (var slot in hoveredSlotsPozitions)
        {
            int slotI = slot.y * inventory.invWidth + slot.x;

            if (slotI < inventory.slots.Count)
            {
                hoveredSlots.Add(inventory.slots[slotI]);
                if (inventory.slots[slotI].taken == false)
                {
                    inventory.slots[slotI].InvSlotImg.color = cGreen;
                }
                else
                {
                    inventory.slots[slotI].InvSlotImg.color = cRed;
                }
            }
        }

        return hoveredSlots;
    }

}
