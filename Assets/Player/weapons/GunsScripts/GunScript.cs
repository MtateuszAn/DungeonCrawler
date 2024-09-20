using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;

public class GunScript : ObjectHoldBehaviour
{
    protected ItemGun itemGun;

    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] protected GameObject muzzleFlash;

    [SerializeField] GameObject WallHitDecal;
    [SerializeField] GameObject BloodHitDecal;
    [SerializeField] protected LayerMask obsticleLayer;

    protected bool gunBusy = false;
    protected bool gunInWall = false;

    override public void InitHand(ItemHold item)
    {
        //Debug.Log("Awake");
        hand.OnShoot += OnShoot;
        hand.OnReload += OnReload;

        if (item is ItemGun) 
        { 
            itemGun = (ItemGun)item;
        }

    }

    protected void SymulateBullet(Vector3 start, Vector3 shootDir, float range , float damage)
    {
        RaycastHit raycastHit;
        GameObject hole;
        if (Physics.Raycast(start, shootDir, out raycastHit, range, obsticleLayer))
        {
            if (raycastHit.transform.gameObject.layer == 9)
            {
                hole = Instantiate(BloodHitDecal, raycastHit.point, WallHitDecal.transform.rotation);
                hole.transform.up = raycastHit.normal;
                hole.transform.parent = raycastHit.transform;
                EnemieScript enemiScript = raycastHit.transform.gameObject.GetComponent<EnemieScript>();
                if (enemiScript != null)
                    enemiScript.takeDamage(damage);
            }
            else
            {
                hole = Instantiate(WallHitDecal, raycastHit.point, WallHitDecal.transform.rotation);
                hole.transform.up = raycastHit.normal;
                hole.transform.parent = raycastHit.transform;

                if(raycastHit.rigidbody != null)
                {
                    Vector3 shootVec = (raycastHit.point - start);
                    shootVec = shootVec.normalized;
                    raycastHit.rigidbody.AddForce(shootVec*damage*10);
                }
            }

        }
    }

    public void OnReloadAnimationEnd()
    {
        gunBusy = false;

        itemGun.courentBulletsInMag = itemGun.maxBuletsInMag;
    }

    public void OnShootAnimationEnd()
    {
        gunBusy = false;
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}
