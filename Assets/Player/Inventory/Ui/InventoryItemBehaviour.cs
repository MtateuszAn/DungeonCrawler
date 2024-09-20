using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItemBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Reference to the Inventory UI item that this behavior is controlling
    public InventoryItem uiInventoryItem;

    // Reference to the inventory UI from which this item originated
    InventoryUIBehaviour orginalInventoryBehaviour;

    // Reference to the Image component for visual representation
    Image image;

    // Slots that are currently hovered over while dragging
    List<UiInventorySlot> hoveredSlots = null;

    // Player input system for handling item rotation during drag
    public PlayerInput playerInput;
    InputAction rotateAction;

    // Flag to track if the item is currently being dragged
    bool isDragging = false;
    // Flag to track if the currently dragged item have been fliped
    bool haveBeenFliped = false;

    private void Start()
    {
        // Find and assign the inventory this item belongs to
        orginalInventoryBehaviour = transform.GetComponentInParent<InventoryUIBehaviour>();

        // Get the player input system from the inventory
        playerInput = orginalInventoryBehaviour.playerInput;

        // Set up the action for rotating the item while dragging
        rotateAction = playerInput.actions["Rotate"];

        // Get the Image component attached to this GameObject
        image = transform.GetComponent<Image>();
    }

    // Called when dragging starts
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Free the slots that were occupied by this item in the original inventory
        uiInventoryItem.FreeSlots();

        // Move this item to the top of the UI hierarchy to ensure it is rendered above other elements
        transform.SetParent(transform.parent.parent, true);
        transform.SetAsLastSibling();

        // Disable raycasting on the image to prevent interaction while dragging
        image.raycastTarget = false;

        // Make the item semi-transparent during drag to indicate it is being moved
        uiInventoryItem.icon.color = new Color(1, 1, 1, 0.5f);

        // Set the dragging flag to true
        isDragging = true;

        // Subscribe to the rotate action to allow rotating the item while dragging
        rotateAction.performed += RotateDraggedItem;
    }

    // Called continuously while the item is being dragged
    public void OnDrag(PointerEventData eventData)
    {
        // Move the item image to follow the mouse position
        image.rectTransform.position = Mouse.current.position.ReadValue();

        // Check if the mouse is currently over an inventory UI
        if (InventoryUIBehaviour.instanceMouseOn != null)
        {
            if (InventoryUIBehaviour.instanceMouseOn.isMouseOver)
                // Update the hovered slots based on the current mouse position
                hoveredSlots = InventoryUIBehaviour.instanceMouseOn.ItemDrag(uiInventoryItem);
        }
    }

    // Called when dragging ends
    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset the item appearance to fully opaque
        uiInventoryItem.icon.color = new Color(1, 1, 1, 1);

        // Re-enable raycasting on the image for normal interactions
        image.raycastTarget = true;

        // Set the dragging flag to false
        isDragging = false;

        // Unsubscribe from the rotate action as dragging has ended
        rotateAction.performed -= RotateDraggedItem;

        // If the mouse is not over any inventory UI
        if (InventoryUIBehaviour.instanceMouseOn == null)
        {
            // Remove the item from its original inventory and destroy the GameObject
            orginalInventoryBehaviour.ItemRemoveFromInv(uiInventoryItem);
            orginalInventoryBehaviour.DropItem(uiInventoryItem.item);
            uiInventoryItem = null;
            //add DropItem
            Destroy(gameObject);
        }
        // If the mouse is over the original inventory
        else if (InventoryUIBehaviour.instanceMouseOn == orginalInventoryBehaviour)
        {
            // Reassign this item to its original parent (the inventory UI)
            transform.SetParent(orginalInventoryBehaviour.transform, true);

            // Attempt to reposition the item within the original inventory
            if(!orginalInventoryBehaviour.ItemChangePosition(uiInventoryItem, hoveredSlots))
            {
                // if item failed to change pozition and have ben fliped, return to orginal rotation and pozition;
                if (haveBeenFliped)
                    uiInventoryItem.Flip();
                uiInventoryItem.UpdateUiPozition();
            }
        }
        // If the mouse is over a different inventory
        else
        {
            // Attempt to add this item to the new inventory
            if (InventoryUIBehaviour.instanceMouseOn.ItemAddfromOtherInventory(this, hoveredSlots))
            {
                // Remove the item from its original inventory and reassign it to the new one
                orginalInventoryBehaviour.ItemRemoveFromInv(uiInventoryItem);
                orginalInventoryBehaviour = InventoryUIBehaviour.instanceMouseOn;
            }
            else
            {
                // If falied restart item pozition and parent 
                if (haveBeenFliped)
                    uiInventoryItem.Flip();
                transform.SetParent(orginalInventoryBehaviour.transform, true);
                uiInventoryItem.UpdateUiPozition();
            }
        }
        //Reset flip condition
        haveBeenFliped = false;
    }

    // Called when the rotate action is performed (e.g., pressing a key to rotate)
    void RotateDraggedItem(InputAction.CallbackContext context)
    {
        // Only rotate if the item is currently being dragged
        if (isDragging)
        {
            haveBeenFliped = !haveBeenFliped;
            // Flip the item to rotate it by 90 degrees
            uiInventoryItem.Flip();

            // Update the hovered slots based on the new orientation of the item
            if (InventoryUIBehaviour.instanceMouseOn != null)
            {
                if (InventoryUIBehaviour.instanceMouseOn.isMouseOver)
                    hoveredSlots = InventoryUIBehaviour.instanceMouseOn.ItemDrag(uiInventoryItem);
            }
        }
    }
}
