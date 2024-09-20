using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun", menuName ="Item/Create New Gun")]
public class ItemGunSO : ItemWeaponSO
{
    [SerializeField] private int maxBuletsInMag;
    [SerializeField] float range;
    [SerializeField] float damage;
    [SerializeField] float spreed;
    [SerializeField] int numberOfBuletsPerShot;

    override public Item NewItem()
    {
        ItemGun newItem = new ItemGun();
        newItem.itemSO = this;
        newItem.itemName = itemName;
        newItem.itemPrefab = itemPrefab;
        newItem.UIwidth = uiWidth;
        newItem.UIheight = uiHeight;
        newItem.icon = icon;
        newItem.itemHoldPrefab = itemHoldPrefab;
        newItem.maxBuletsInMag = maxBuletsInMag;
        newItem.courentBulletsInMag = maxBuletsInMag;
        newItem.range = range;
        newItem.damage = damage;
        newItem.spreed = spreed;
        newItem.numberOfBuletsPerShot = numberOfBuletsPerShot;
        return newItem;
    }
}
