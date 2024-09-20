using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSeatBehaviour : Interactable
{
    public override string GetText()
    {
        return "Sit in car";
    }
    public override bool Interact()
    {
        PlayerStateManager.Instance.GetInCar(this);
        return true;
    }
}
