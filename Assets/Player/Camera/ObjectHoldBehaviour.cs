using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHoldBehaviour : MonoBehaviour
{
    public HandBehaviour hand;

    virtual public void InitHand(ItemHold item)
    {
        Debug.Log("Awake");
        hand.OnShoot += OnShoot;
        hand.OnReload += OnReload;
    }

    private void OnEnable()
    {
        if (hand != null)
        {
            hand.OnShoot += OnShoot;
            hand.OnReload += OnReload;
        }
    }
    private void OnDisable()
    {
        //Debug.Log("Disable");
        if(hand != null)
        {
            hand.OnShoot -= OnShoot;
            hand.OnReload -= OnReload;
        }
        
    }
    virtual protected void OnShoot()
    {

    }
    virtual protected void OnReload()
    {

    }
}
