using UnityEngine;
using UnityEngine.InputSystem;

public class BeltManager : MonoBehaviour
{
    PlayerInput input;
    InputAction action1;
    InputAction action2;
    InputAction action3;
    InputAction action4;
    InputAction action5;

    [SerializeField] HandBehaviour handBehaviour;

    [SerializeField]public InventoryBeltSlotBehaviour[] inventoryBeltSlots = new InventoryBeltSlotBehaviour[5];

    int courentSlot = -1;

    private void Awake()
    {
        // Inicjalizacja PlayerInput oraz akcji
        input = GetComponent<PlayerInput>();
        action1 = input.actions["Slot1"];
        action2 = input.actions["Slot2"];
        action3 = input.actions["Slot3"];
        action4 = input.actions["Slot4"];
        action5 = input.actions["Slot5"];

        // Przypisanie metod callback do akcji
        action1.performed += ctx => OnBeltAction(0);
        action2.performed += ctx => OnBeltAction(1);
        action3.performed += ctx => OnBeltAction(2);
        action4.performed += ctx => OnBeltAction(3);
        action5.performed += ctx => OnBeltAction(4);
    }

    private void OnEnable()
    {
        // W³¹czamy akcje przy aktywowaniu obiektu
        action1.Enable();
        action2.Enable();
        action3.Enable();
        action4.Enable();
        action5.Enable();
    }

    private void OnDisable()
    {
        // Wy³¹czamy akcje przy deaktywacji obiektu
        action1.Disable();
        action2.Disable();
        action3.Disable();
        action4.Disable();
        action5.Disable();
    }

    // Metoda wywo³ywana, gdy którakolwiek z akcji zostanie wykonana
    private void OnBeltAction(int slotNumber)
    {
        if (slotNumber == courentSlot)
            return;

        courentSlot = slotNumber;
        if (
            inventoryBeltSlots[slotNumber].items != null 
            && inventoryBeltSlots[slotNumber].items.Count != 0 
            && inventoryBeltSlots[slotNumber].items[0] != null
            && inventoryBeltSlots[slotNumber].items[0].item is ItemHold item)
        {
            handBehaviour.ChangeItemInHand(item);
        }
        else
        {
           handBehaviour.FreeHand();
        }
    }
}
