using System.Collections.Generic;
using UnityEngine;


public class InventoryBehaviour : MonoBehaviour
{
    //UiInventory
    [SerializeField] public InventoryUIBehaviour inventoryUI;

    // Inventory configuration
    [SerializeField]public int invWidth = 10;  // Width of the inventory grid
    [SerializeField]public int invHeight = 10;  // Height of the inventory grid
    public static int tileSize = 50;  // Size of each grid cell

    public List<UiInventorySlot> slots = new List<UiInventorySlot>();  // List of all slots in the inventory
    public List<InventoryItem> items = new List<InventoryItem>();  // List of all items in the inventory

    public static Vector2 lastScreenSize;
    private void Awake()
    {
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        tileSize = (int)Mathf.Lerp(0f, 50, Screen.height/998f);
        CreateSlots();
    }

    void OnRectTransformDimensionsChange()
    {
        // Tutaj mo¿esz umieœciæ kod, który ma byæ wywo³any przy zmianie rozmiaru okna
        Debug.Log("Rozmiar okna zosta³ zmieniony");
    }
    public void CreateSlots()
    {
        slots.Clear();
        for (int j = 0; j < invHeight; j++)
        {
            for (int i = 0; i < invWidth; i++)
            {
                UiInventorySlot newSlot = new UiInventorySlot
                {
                    x = i,
                    y = j,
                    InvSlotImg = null
                };
                slots.Add(newSlot);
            }
        }
    }

    #region AddItem
    // Attempts to add a new item to the inventory
    public bool TryToAddToInventory(Item newItem)
    {
        InventoryItem item = new InventoryItem(newItem);
        Vector2Int poz = Vector2Int.zero;
        List<UiInventorySlot> itemSlots;

        // Try to find a valid position for the item in the grid
        for (int j = 0; j < invHeight; j++)
            for (int i = 0; i < invWidth; i++)
            {
                poz.x = i;
                poz.y = j;
                itemSlots = ValidateItemInPozition(item, poz);

                if (itemSlots != null)
                    goto Sucess;
            }

        // If no valid position is found, try rotating the item and searching again
        item.Flip();
        for (int j = 0; j < invHeight; j++)
            for (int i = 0; i < invWidth; i++)
            {
                poz.x = i;
                poz.y = j;
                itemSlots = ValidateItemInPozition(item, poz);

                if (itemSlots != null)
                    goto Sucess;
            }

        return false;

    Sucess:
        // If a valid position is found, place the item in the inventory
        item.SetUpItem(poz, itemSlots);
        item.TakeSlots();
        if(inventoryUI!=null && inventoryUI.isActiveAndEnabled)
            inventoryUI.CreateItemUi(item);
        items.Add(item);

        return true;
    }

    // Validates if an item can be placed at a specific position in the grid
    public List<UiInventorySlot> ValidateItemInPozition(InventoryItem item, Vector2Int poz)
    {
        List<UiInventorySlot> itemSlots = new List<UiInventorySlot>();

        for (int j = 0; j < item.height; j++)
            for (int i = 0; i < item.width; i++)
            {
                int slotI = (j + poz.y) * invWidth + (i + poz.x);

                if (i + poz.x >= invWidth || j + poz.y >= invHeight || i + poz.x < 0 || j + poz.y < 0)
                    return null;

                if (slots[slotI].taken)
                    return null;

                itemSlots.Add(slots[slotI]);
            }

        return itemSlots;
    }
    public bool LoadItems(InventoryItemData itemData)
    {
        InventoryItem item = new InventoryItem(itemData.item.NewItem());
        Vector2Int poz = new Vector2Int(itemData.x, itemData.y);
        List<UiInventorySlot> itemSlots = ValidateItemInPozition(item, poz);

        if(itemSlots == null)
            return false;
           
        item.SetUpItem(poz, itemSlots);
        item.TakeSlots();
        if (inventoryUI != null && inventoryUI.isActiveAndEnabled)
            inventoryUI.CreateItemUi(item);
        items.Add(item);

        return true;
    }
    #endregion
}
