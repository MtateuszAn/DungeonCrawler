using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandBehaviour : MonoBehaviour
{
    ItemHold itemObject;
    GameObject itemInstace;

    Animator animator;
    [SerializeField] PlayerInput playerInput;
    InputAction shotAction;
    InputAction reloadAction;

    Transform handTransform;

    public event System.Action OnShoot;
    public event System.Action OnReload;

    void Start()
    {
        handTransform = transform.GetChild(0).transform;
        animator = GetComponentInChildren<Animator>();
        shotAction = playerInput.actions["PlayerShoot"];
        reloadAction = playerInput.actions["PlayerReload"];

        shotAction.started += ctx => OnShoot?.Invoke();
        reloadAction.started += ctx => OnReload?.Invoke();
    }

    public void ChangeItemInHand(ItemHold item)
    {
        animator.Play("HandStartItemAnimation");
        if (itemInstace != null) { Destroy(itemInstace); }
        //Debug.Log(item.itemName);
        itemObject = item;
        itemInstace = Instantiate(item.itemHoldPrefab, handTransform);
        itemInstace.GetComponent<ObjectHoldBehaviour>().hand=this;
        itemInstace.GetComponent<ObjectHoldBehaviour>().InitHand(item);
    }

    public void FreeHand()
    {
        Debug.Log("Hand Has ben freed");
        Destroy(itemInstace);
        itemObject = null;
    }


}
