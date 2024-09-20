using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemGun : ItemWeapon
{
    public int maxBuletsInMag;
    public int courentBulletsInMag;

    public float range;
    public float damage;
    public float spreed;
    public int numberOfBuletsPerShot;
}
