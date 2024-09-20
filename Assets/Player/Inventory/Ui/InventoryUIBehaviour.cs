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
// Class representing each slot in the inventory
// Determines if the slot is occupied by an item
public class UiInventorySlot
{
    public int x, y;  // Position of the slot in the inventory grid
    public Image InvSlotImg;  // Image component to visualize the slot
    public bool taken = false;  // Whether the slot is occupied by an item
}

public class InventoryUIBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Transform dropPoint;

    public InventoryBehaviour inventory;
    public static InventoryUIBehaviour instanceMouseOn;  // Tracks the current inventory under the mouse

    // Inventory UI components
    [HideInInspector] private RectTransform rectTransform;  // RectTransform for the inventory UI
    [SerializeField] protected Image InvSlotImg;  // Template for slot images
    [SerializeField] protected Image ItemTemplate;  // Template for item images

    // Test variables (for debugging)
    [SerializeField] protected TMP_Text testText;

    // Slot highlight colors
    protected Color cRed = new Color(0.6f, 0, 0, 0.5f);  // Red for occupied slots
    protected Color cGreen = new Color(0, 0.6f, 0, 0.5f);  // Green for available slots
    protected Color cWhite = new Color(0.6f, 0.6f, 0.6f, 0.3f);  // Default white color

    // Mouse-related variables
    [HideInInspector] public bool isMouseOver = false;  // Whether the mouse is over the inventory
    protected Vector2Int gridMousePoint = Vector2Int.zero;  // Mouse position in grid coordinates

    // Input system
    [SerializeField] public PlayerInput playerInput;

    #region Initiate
    // Start is called before the first frame update
    void Start()
    {
        // Initialization can go here if needed
    }

    protected void OnEnable()
    {
        // Initialize RectTransform and set size based on inventory dimensions
        rectTransform = GetComponent<RectTransform>();
        //rectTransform.sizeDelta = new Vector2(inventory.invWidth * InventoryBehaviour.tileSize, inventory.invHeight * InventoryBehaviour.tileSize);
        if (inventory is InventoryConteinerBehaviour)
            rectTransform.position = new Vector3((-(inventory.invWidth * InventoryBehaviour.tileSize) - 21) + Screen.width, -83f + Screen.height, 0f);
        // Create visual representations of the inventory slots
        InstantiateSlotsImages();
        ClearColor();

        // Set up UI for existing items
        foreach (InventoryItem item in inventory.items)
        {
            CreateItemUi(item);
            item.ChengePozition(GetItemGrid(item, new Vector2Int(item.x, item.y)));
        }

        isMouseOver = false;  // Reset mouse-over status

        if (inventory is InventoryConteinerBehaviour conteiner)
        {
            //conteiner.inventoryUI.rectTransform.position = new Vector3((-(conteiner.invWidth * InventoryBehaviour.tileSize) - 21) + Screen.width, -83f + Screen.height, 0f);
            conteiner.OnInvUiLink();
        }
    }

    protected void OnDisable()
    {
        // Clean up slot images when inventory is disabled
        foreach (UiInventorySlot slot in inventory.slots)
        {
            Destroy(slot.InvSlotImg.gameObject);
        }
        foreach (InventoryItem item in inventory.items)
        {
            item.CleerItemVisuals();
        }
        foreach (UiInventorySlot slot in inventory.slots)
        {
            slot.InvSlotImg=null;
        }

        if(inventory is InventoryConteinerBehaviour conteiner)
        {
            conteiner.OnInvUiUnlink();
        }
    }

    // Instantiate slot images and position them in the grid
    void InstantiateSlotsImages()
    {
        for (int j = 0; j < inventory.invHeight; j++)
        {
            for (int i = 0; i < inventory.invWidth; i++)
            {
                int slotI = j * inventory.invWidth + i;

                inventory.slots[slotI].InvSlotImg = Instantiate(InvSlotImg, gameObject.transform);

                inventory.slots[slotI].InvSlotImg.rectTransform.localPosition = new Vector2(i * InventoryBehaviour.tileSize, -j * InventoryBehaviour.tileSize);
                inventory.slots[slotI].InvSlotImg.rectTransform.sizeDelta = new Vector2(InventoryBehaviour.tileSize, InventoryBehaviour.tileSize);
            }
        }
    }

    // Reset the color of all slots to the default white
    protected void ClearColor()
    {
        foreach (UiInventorySlot slot in inventory.slots)
        {
            slot.InvSlotImg.color = cWhite;
        }
    }

    // Create UI elements for a new item
    public void CreateItemUi(InventoryItem item)
    {
        item.SetUpItemVisuals(Instantiate(ItemTemplate, gameObject.transform), InventoryBehaviour.tileSize);
    }

    public List<UiInventorySlot> GetItemGrid(InventoryItem item, Vector2Int poz)
    {
        List<UiInventorySlot> itemSlots = new List<UiInventorySlot>();

        for (int j = 0; j < item.height; j++)
            for (int i = 0; i < item.width; i++)
            {
                int slotI = (j + poz.y) * inventory.invWidth + (i + poz.x);

                if (i + poz.x >= inventory.invWidth || j + poz.y >= inventory.invHeight || i + poz.x < 0 || j + poz.y < 0)
                    return null;

                itemSlots.Add(inventory.slots[slotI]);
            }

        return itemSlots;
    }

    #endregion

    #region Mouse
    // Update is called once per frame
    void Update()
    {
        testText.text = inventory.items.Count.ToString();  // Update item count text (for debugging)

        if (isMouseOver)
        {
            MousePositionUpdate();  // Update mouse position within the grid
        }

        if (Screen.width != InventoryBehaviour.lastScreenSize.x || Screen.height != InventoryBehaviour.lastScreenSize.y)
        {
            if (inventory.inventoryUI != null)
            {
                inventory.inventoryUI.enabled = false;
                inventory.inventoryUI.enabled = true;
            }
        }

    }

    // Updates the mouse position relative to the inventory grid
    private void MousePositionUpdate()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();  // Get the mouse position in screen coordinates

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            mousePosition,
            null,
            out Vector2 localMousePoint
        );

        gridMousePoint = MouseToGrid(localMousePoint);  // Convert to grid coordinates
    }

    // Converts a local mouse position to grid coordinates
    Vector2Int MouseToGrid(Vector2 mousePoz)
    {
        Vector2Int gridPoz = Vector2Int.zero;

        gridPoz.x = Math.Clamp((int)Math.Floor(mousePoz.x / InventoryBehaviour.tileSize), 0, inventory.invWidth);
        gridPoz.y = Math.Clamp(-(int)Math.Floor(mousePoz.y / InventoryBehaviour.tileSize) - 1, 0, inventory.invHeight);

        return gridPoz;
    }

    // Handles mouse entering the inventory area
    public void OnPointerEnter(PointerEventData eventData)
    {
        cWhite = new Color(0.6f, 0.7f, 0.6f, 0.3f);
        ClearColor();
        isMouseOver = true;
        instanceMouseOn = this;
    }

    // Handles mouse exiting the inventory area
    public void OnPointerExit(PointerEventData eventData)
    {
        cWhite = new Color(0.6f, 0.6f, 0.6f, 0.3f);
        ClearColor();
        isMouseOver = false;
        instanceMouseOn = null;
    }
    #endregion

    #region DragItem
    // Handles the dragging of an item across the inventory
    virtual internal List<UiInventorySlot> ItemDrag(InventoryItem itemDragged)
    {
        ClearColor();  // Clear slot highlights

        int x = gridMousePoint.x - itemDragged.width / 2;
        int y = gridMousePoint.y - itemDragged.height / 2;

        List<Vector2Int> hoveredSlotsPozitions = new List<Vector2Int>();

        for (int j = 0; j < itemDragged.height; j++)
        {
            for (int i = 0; i < itemDragged.width; i++)
            {
                hoveredSlotsPozitions.Add(new Vector2Int(x + i, y + j));

                if (i + x >= inventory.invWidth || j + y >= inventory.invHeight || i + x < 0 || j + y < 0)
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

    // Handles the final placement of an item after dragging
    internal bool ItemChangePosition(InventoryItem itemDragged, List<UiInventorySlot> hoveredSlots)
    {
        ClearColor();

        bool canBePlaced = true;

        if (hoveredSlots != null)
        {
            foreach (UiInventorySlot slot in hoveredSlots)
            {
                if (slot.taken)
                    canBePlaced = false;
            }

            if (canBePlaced)
            {
                itemDragged.ChengePozition(hoveredSlots);  // ItemDropSuccess, change pozition
                return true;
            }
            else
            {
                return false;// ItemDropFail
            }
                
        }
        else
        {
            return false;// ItemDropFail
        }
            
    }

    // Adds an item from another inventory if possible
    internal bool ItemAddfromOtherInventory(InventoryItemBehaviour itemDragged, List<UiInventorySlot> hoveredSlots)
    {
        ClearColor();
        bool canBePlaced = true;

        if (hoveredSlots != null)
        {
            foreach (UiInventorySlot slot in hoveredSlots)
            {
                if (slot.taken)
                    canBePlaced = false;
            }

            if (canBePlaced)
            {
                itemDragged.transform.SetParent(gameObject.transform, false);
                itemDragged.uiInventoryItem.ChengePozition(hoveredSlots);  // ItemDropSuccess
                inventory.items.Add(itemDragged.uiInventoryItem);
                return true;
            }
            else
            {
                // ItemDropFail
                return false;
            }
        }
        else
        {
            // ItemDropFail
            return false;
        }
    }

    // Removes an item from the inventory
    internal void ItemRemoveFromInv(InventoryItem uiInventoryItem)
    {
        inventory.items.Remove(uiInventoryItem);
        uiInventoryItem = null;
    }

    internal void DropItem(Item item)
    {
        GameObject instItem = Instantiate(item.itemPrefab,dropPoint.position,dropPoint.rotation);
        instItem.GetComponent<Rigidbody>().AddForce(dropPoint.forward*2f,ForceMode.Impulse);
        instItem.GetComponent<SmalItemBehaviour>().SetItem(item);
        if(ItemsParentsControllerBehaviour.Instance != null)
            instItem.transform.parent = ItemsParentsControllerBehaviour.Instance.smallItemsParent;
        else
            instItem.transform.parent = null;
    }
    #endregion

}
