using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LargeItemBehaviour : ItemBehaviour
{
    [SerializeField] private string LargeitemName;
    [SerializeField] public LargeItemSO item;
    protected override string itemName
    {
        get { return LargeitemName; }
        set { LargeitemName = value; }
    }
}
