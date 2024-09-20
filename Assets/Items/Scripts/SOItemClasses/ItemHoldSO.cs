using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHoldSO : ItemSO
{
    [Tooltip("Prefab when holding item in hand")]
    [SerializeField] protected GameObject itemHoldPrefab;

    override public Item NewItem()
    {
        ItemHold newItem = new ItemHold();
        newItem.itemSO = this;
        newItem.itemName = itemName;
        newItem.itemPrefab = itemPrefab;
        newItem.UIwidth = uiWidth;
        newItem.UIheight = uiHeight;
        newItem.icon = icon;
        newItem.itemHoldPrefab= itemHoldPrefab;
        return newItem;
    }
}
