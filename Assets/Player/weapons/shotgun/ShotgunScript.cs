using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShotgunScript : GunScript
{
    [SerializeField] Transform barrelL;
    [SerializeField] Transform barrelR;
    Transform barrelStart;
    Transform barrelEnd;
    

    [SerializeField] Animator wholeGunAnimator;
    Animator animator;
    InputAction shotAction;
    InputAction reloadAction;




    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        //shotAction = playerInput.actions["PlayerShoot"];
        //reloadAction = playerInput.actions["PlayerReload"];

        //barrelStart = GameObject.Find("BarrelStart").transform;
        //barrelEnd = GameObject.Find("BarrelEnd").transform;

        //shotAction.started += ctx => Shoot();
        //reloadAction.started += ctx => Reload();
    }
    /*private void Update()
    {
        Debug.DrawLine(barrelStart.position, barrelEnd.position);
        if (Physics.Linecast(barrelStart.position, barrelEnd.position, obsticleLayer))
        {
            //Debug.Log("In WALL");
            gunInWall = true;
            wholeGunAnimator.SetBool("InWall", true);
        }
        else
        {
            //Debug.Log("Cleer");
            gunInWall = false;
            wholeGunAnimator.SetBool("InWall", false);
        }
    }*/
    override protected void OnReload()
    {
        if (!gunBusy)
        {
            gunBusy = true;
            animator.SetTrigger("Reload");
        }
    }

    override protected void OnShoot()
    {
        if (!gunBusy && !gunInWall && itemGun.courentBulletsInMag>0)
        {
            gunBusy = true;
            Vector3 spred;
            Debug.Log(itemGun.numberOfBuletsPerShot);
            for (int i = 0; i < itemGun.numberOfBuletsPerShot; i++) 
            {
                Debug.Log("BULLET SHOT");
                spred = new Vector3(Random.Range(-itemGun.spreed, itemGun.spreed), Random.Range(-itemGun.spreed, itemGun.spreed), Random.Range(-itemGun.spreed, itemGun.spreed));
                //Parent function drawing ray for bullet hits
                SymulateBullet(barrelL.position, gameObject.transform.forward + spred, itemGun.range, itemGun.damage / itemGun.numberOfBuletsPerShot);
            }
            
            animator.SetTrigger("Shot");

            GameObject flash;
            switch (itemGun.courentBulletsInMag)
            {
                case 2:
                    flash = Instantiate(muzzleFlash, barrelL.position, barrelL.rotation, barrelL);
                    Destroy(flash, 0.1f);
                    break; 
                case 1:
                    flash = Instantiate(muzzleFlash, barrelR.position, barrelR.rotation, barrelR);
                    Destroy(flash, 0.1f);
                    break;
            }
            itemGun.courentBulletsInMag--;
        }
    }




}
