using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetOutBehaviour : Interactable
{
    public override string GetText()
    {
        return "Leave car";
    }
    public override bool Interact()
    {
        if(CarBehaviour.Instance.inCar)
        {
            PlayerStateManager.Instance.GetOutOfCar(this);
            return true;
        }
        gameObject.SetActive(false);
        return false;
    }
    public Vector3 ReycastDown()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2))
        {
            return hit.point;
        }
        return transform.position- new Vector3(0,-1,0);
    }
}
