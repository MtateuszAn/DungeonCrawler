using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName ="Item/Create New Item")]
public class ItemSO : ScriptableObject
{
    
    [Header("Item Info")]
    [SerializeField] protected int itemID;
    [SerializeField] protected string itemName;

    [Header("Ui variables")]
    [SerializeField] protected Sprite icon;
    [SerializeField][Range(1, 9)] protected int uiWidth, uiHeight;

    [Header("Item Prefab")]
    [SerializeField] public GameObject itemPrefab;
    virtual public Item NewItem()
    {
        Item newItem = new Item();
        newItem.itemSO = this;
        newItem.itemName = itemName;
        newItem.itemPrefab = itemPrefab;
        newItem.UIwidth = uiWidth;
        newItem.UIheight = uiHeight;
        newItem.icon = icon;
        return newItem;
    }
}
