using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteractBehaviour : MonoBehaviour
{
    [SerializeField] InventoryBehaviour inventory;
    [SerializeField] Transform cameraT;
    [SerializeField] LayerMask interactLeyerMask;
    [SerializeField] float interactRange;
    [SerializeField] GameObject interactUI;
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] Image interactFillBar;
    [SerializeField] InventoryUIBehaviour inventoryUIBehaviour;
    [SerializeField] UiManagerBehaviour uiManagerBehaviour;
    public ItemBehaviour itemDetected { get; private set;}
    public InventoryBehaviour inventoryDetected { get; private set; }
    public Interactable interactableDetected { get; private set; }

    [SerializeField]bool detected;

    InventoryBehaviour inventoryBehaviour;

    
    PlayerInput playerInput;
    InputAction interactAction;

    public RaycastHit hit;

    private void Start()
    {
        inventory = GetComponent<InventoryBehaviour>();
        inventoryBehaviour = GetComponent<InventoryBehaviour>();
        playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"];
        interactAction.performed += ctx => Interact();
        interactFillBar.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        interactUI.SetActive(false);
        if (Physics.Raycast(cameraT.position, cameraT.forward, out hit, interactRange, interactLeyerMask))
        {
            detected = false;
            //Small items to add to inventory and small and large item to move around
            if (hit.collider.gameObject.TryGetComponent<ItemBehaviour>(out ItemBehaviour item))
            {
                itemDetected = item;
                interactUI.SetActive(true);
                itemNameText.text = itemDetected.GetItemName();
                detected = true;
            }
            else
            {
                itemDetected = null;
            }
            //Interactable like Doors
            if (hit.collider.gameObject.TryGetComponent<Interactable>(out Interactable interactable))
            {
                interactUI.SetActive(true);
                itemNameText.text = interactable.GetText();
                interactableDetected = interactable;
                detected = true;

            }
            else if (hit.collider.gameObject.TryGetComponent<InventoryBehaviour>(out InventoryBehaviour inventory))//External inventories like chests
            {
                inventoryDetected = inventory;
                interactUI.SetActive(true);
                itemNameText.text = "OPEN";
                detected = true;
            }
            else
            {
                itemNameText.text = string.Empty;
                inventoryDetected = null;
                interactableDetected = null;
            }
        }
        else
        {
            //need some optymalization
            itemNameText.text = string.Empty;
            itemDetected = null;
            inventoryDetected = null;
            detected = false ;
        }
    }

    void Interact()
    {
        StopAllCoroutines();
        StartCoroutine(InteractCoreRutine(this));
    }
    IEnumerator InteractCoreRutine(PlayerInteractBehaviour instance)
    {
        interactFillBar.gameObject.SetActive(true);
        interactFillBar.fillAmount = 0;
        float i = 0f;
        while (i < 1f)
        {
            interactFillBar.fillAmount = i;
            i += Time.deltaTime*2;
            yield return new WaitForEndOfFrame();
            if (!detected)
            {
                interactFillBar.gameObject.SetActive(false);
                yield break;
            }
        }
        interactFillBar.gameObject.SetActive(false);
        //item Interactions
        if (inventoryDetected != null)
        {
            //Debug.Log("smallItemDetected ");
            inventoryUIBehaviour.inventory = inventoryDetected;
            inventoryDetected.inventoryUI = inventoryUIBehaviour;
            uiManagerBehaviour.OpenExternalInventory();
        }else if (itemDetected != null)
        {
            if (itemDetected is SmalItemBehaviour smalItem)//if it can be put in inventory
            {
                //Debug.Log("smallItemDetected ");
                if (inventory.TryToAddToInventory(smalItem.GetItem()))
                {
                    smalItem.DestroyItem();
                }
                else
                {
                    //Debug.Log("no space in inventory for: " + smalItem.GetItemName());
                    UiPopUpManager.Instance.CreatePopUp("no space in inventory for: " + smalItem.GetItemName());
                }
            }
        }else if (interactableDetected != null)
        {
            interactableDetected.Interact();
        }
    }
}
