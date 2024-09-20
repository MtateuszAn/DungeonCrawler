using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmalItemBehaviour : ItemBehaviour
{
    [SerializeField] public ItemSO itemSO;
    Item item;

    private void Start()
    {
        if (item == null && itemSO != null)
        {
            item = itemSO.NewItem();
        }
            

        if (item != null)
            itemName = item.itemName;
        else
            Debug.LogError("ERROR no item assigned to" + name);
    }
    public void SetItem(Item newItem)
    {
        item = newItem;
    }
    public Item GetItem()
    {
        return item;
    }
    public void DestroyItem()
    {
        Destroy(gameObject);
    }

}
