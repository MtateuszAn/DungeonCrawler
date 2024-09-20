using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiManagerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject playerInventory;
    [SerializeField] private GameObject externalInventory;
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private PlayerInput playerMovmentInput;
    [SerializeField] PlayerStateManager playerStateManager;

    PlayerInput playerInput;//player input system
    InputAction inventoryButton;//button for inventory
    InputAction menuButton;//button for closing any open window or opening in game menu

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        inventoryButton = playerInput.actions["Inventory"];
        menuButton = playerInput.actions["Esc"];

        inventoryButton.performed += OpenCloseInventory;
        menuButton.performed += CloseInventoryOrOpenMenu;
        CloseInventory();
    }
    private void OnDestroy()
    {
        inventoryButton.performed -= OpenCloseInventory;
        menuButton.performed -= CloseInventoryOrOpenMenu;
    }
    private void OpenCloseInventory(InputAction.CallbackContext obj)
    {
        if (!playerInventory.activeInHierarchy)
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }
    //IngameMenu Handeler, when inventory opened esc closes it
    private void CloseInventoryOrOpenMenu(InputAction.CallbackContext obj)
    {
        if (playerInventory.activeInHierarchy)
        {
            CloseInventory();
        }
        else
        {
            if (!inGameMenu.activeInHierarchy)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
    }
    public void OpenExternalInventory()
    {
        if (!inGameMenu.activeInHierarchy)
        {
            playerInventory.SetActive(true);
            externalInventory.SetActive(true);
            LockMovment();
            Cursor.lockState = CursorLockMode.None;
        }
    }
    void OpenInventory()
    {
        if (!inGameMenu.activeInHierarchy)
        {
            playerInventory.SetActive(true);
            LockMovment();
            Cursor.lockState = CursorLockMode.None;
        }
    }
    void CloseInventory()
    {
        playerInventory.SetActive(false);
        externalInventory.SetActive(false);
        UnlockMovment();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void OpenMenu()
    {
        inGameMenu.SetActive(true);
        LockMovment();
        Cursor.lockState = CursorLockMode.Confined;
    }
    void CloseMenu()
    {
        inGameMenu.SetActive(false);
        UnlockMovment();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LockMovment()
    {
        playerMovmentInput.enabled = false;
        playerStateManager.canLook = false;
        playerStateManager.canMove = false;
    }

    void UnlockMovment()
    {
        playerMovmentInput.enabled = true;
        playerStateManager.canLook = true;
        playerStateManager.canMove = true;
    }
}
