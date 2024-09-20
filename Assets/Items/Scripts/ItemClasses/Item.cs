using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public ItemSO itemSO;
    public string itemName;
    public GameObject itemPrefab;
    public int UIwidth, UIheight;
    public Sprite icon;
}
