using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShotgunScript : MonoBehaviour
{

    [SerializeField] PlayerInput playerInput;
    [SerializeField] Transform barrelL;
    [SerializeField] Transform barrelR;
    Transform barrelStart;
    Transform barrelEnd;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject pint;
    [SerializeField] GameObject pintBlod;
    [SerializeField] float range;
    [SerializeField] float damage;
    [SerializeField] float spreed;
    [SerializeField] int numberOfBulets;
    [SerializeField] LayerMask obsticleLayer;
    [SerializeField] Animator wholeGunAnimator;
    Animator animator;
    InputAction shotAction;
    InputAction reloadAction;


    bool gunBusy=false;
    bool gunInWall=false;

    int amunition = 2;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        shotAction = playerInput.actions["PlayerShoot"];
        reloadAction = playerInput.actions["PlayerReload"];

        barrelStart = GameObject.Find("BarrelStart").transform;
        barrelEnd = GameObject.Find("BarrelEnd").transform;

        shotAction.started += ctx => Shoot();
        reloadAction.started += ctx => Reload();
    }
    private void Update()
    {
        Debug.DrawLine(barrelStart.position, barrelEnd.position);
        if (Physics.Linecast(barrelStart.position, barrelEnd.position, obsticleLayer))
        {
            gunInWall = true;
            wholeGunAnimator.SetBool("InWall", true);
        }
        else
        {
            gunInWall = false;
            wholeGunAnimator.SetBool("InWall", false);
        }
    }
    private void Reload()
    {
        if (!gunBusy)
        {
            gunBusy = true;
            animator.SetTrigger("Reload");
        }
    }

    private void Shoot()
    {
        if (!gunBusy && !gunInWall && amunition>0)
        {
            RaycastHit raycastHit;
            gunBusy = true;
            GameObject hole;
            Vector3 spred;
            for (int i = 0; i < numberOfBulets; i++) 
            {
                spred = new Vector3(Random.Range(-spreed,spreed), Random.Range(-spreed, spreed), Random.Range(-spreed, spreed));
                if (Physics.Raycast(barrelL.position, gameObject.transform.forward + spred, out raycastHit, range, obsticleLayer))
                {
                    if (raycastHit.transform.gameObject.layer==9)
                    {
                        hole = Instantiate(pintBlod, raycastHit.point, pint.transform.rotation);
                        hole.transform.up = raycastHit.normal;
                        hole.transform.parent = raycastHit.transform;
                        raycastHit.transform.gameObject.GetComponent<EnemieScript>().takeDamage(damage / numberOfBulets);
                    }
                    else
                    {
                        hole = Instantiate(pint, raycastHit.point, pint.transform.rotation);
                        hole.transform.up = raycastHit.normal;
                        hole.transform.parent = raycastHit.transform;
                    }

                }
            }
            
            animator.SetTrigger("Shot");
            GameObject flash;
            switch (amunition)
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
            amunition --;
        }
    }

    public void OnReloadAnimationEnd()
    {
        gunBusy = false;
            amunition = 2;
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
