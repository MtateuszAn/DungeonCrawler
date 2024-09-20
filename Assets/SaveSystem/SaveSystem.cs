using System.Collections.Generic;
using UnityEngine;


public static class SaveSystem
{
    public static SaveGameData SaveGame()
    {
        SaveGameData data = new SaveGameData
        {
            playerData = SavePlayerData(),
            worldItemsData = SaveWorldItemsData(),
            carData = SaveCarData(),
        };
        return data;
    }
    public static ExpeditionData SaveExpedition()
    {
        ExpeditionData data = new ExpeditionData
        {
            playerData = SavePlayerData(),
            carData = SaveCarData(),
        };
        return data;
    }
    public static WorldItemsData SaveHome()
    {
        return SaveWorldItemsData();
    }
    #region Player
    private static PlayerData SavePlayerData()
    {
        List<InventoryData> slotsData = new List<InventoryData>();

        foreach (InventoryBehaviour slot in PlayerStateManager.Instance.beltManager.inventoryBeltSlots)
        {
            InventoryData data = SaveInventoryData(slot);
            slotsData.Add(data);
        }

        PlayerData playerData = new PlayerData
        {
            position = PlayerStateManager.Instance.transform.position,
            cameraRotation = PlayerStateManager.Instance.cameraM.look,
            inventoryData = SaveInventoryData(PlayerStateManager.Instance.GetComponent<InventoryBehaviour>()),
            slotsData = slotsData,
        };
        return playerData;
    }
    #endregion
    #region Inventory
    private static InventoryData SaveInventoryData(InventoryBehaviour inventoryBehaviour)
    {
        InventoryData inventoryData = new InventoryData
        {
            invHeight= inventoryBehaviour.invHeight,
            invWidth=inventoryBehaviour.invWidth,
            items = SaveInventoryItemList(inventoryBehaviour),
        };
        return inventoryData;
    }
    private static List<InventoryItemData> SaveInventoryItemList(InventoryBehaviour inventoryBehaviour)
    {
        List<InventoryItemData> items = new List<InventoryItemData>();
        foreach (InventoryItem invItem in inventoryBehaviour.items)
        {
            InventoryItemData newItemData = new InventoryItemData
            {
                item = invItem.item.itemSO,
                y = invItem.y,
                x = invItem.x,
            };

            items.Add(newItemData);
        }
        return items;
    }
    #endregion
    #region CarItems
    private static CarData SaveCarData()
    {
        CarData carData = new CarData
        {
            position = CarBehaviour.Instance.transform.localPosition,
            rotation = CarBehaviour.Instance.transform.rotation,
            carItemsData = SaveCarItemsData(),
            playerInCar = CarBehaviour.Instance.inCar,
        };
        return carData;
    }

    private static WorldItemsData SaveCarItemsData()
    {
        Debug.Log("In car detected " + CarBehaviour.Instance.smallItemsParent.childCount + " items");
        WorldItemsData worldItemsData = new WorldItemsData
        {
            SmallItemsData = SaveSmallItemsData(CarBehaviour.Instance.smallItemsParent),
            LargeItemsData = SaveLargeItemsData(CarBehaviour.Instance.largeItemsParent),
            LargeContainerData = SaveLargeContainersData(CarBehaviour.Instance.largeContainerParent)
        };
        return worldItemsData;
    }
    #endregion
    #region WorldItems
    private static WorldItemsData SaveWorldItemsData()
    {
        Debug.Log("In world detected " + ItemsParentsControllerBehaviour.Instance.smallItemsParent.childCount + " items");
        WorldItemsData worldItemsData = new WorldItemsData
        {
            SmallItemsData = SaveSmallItemsData(ItemsParentsControllerBehaviour.Instance.smallItemsParent),
            LargeItemsData = SaveLargeItemsData(ItemsParentsControllerBehaviour.Instance.largeItemsParent),
            LargeContainerData = SaveLargeContainersData(ItemsParentsControllerBehaviour.Instance.largeContainerParent)
        };
        return worldItemsData;
    }
    
    private static List<SmallItemData> SaveSmallItemsData(Transform parent)
    {
        List<SmallItemData> itemDatas = new List<SmallItemData>();
        foreach(Transform child in parent)
        {
            SmallItemData smallItemData = new SmallItemData 
            {
                item = child.GetComponent<SmalItemBehaviour>().itemSO,
                position = child.transform.localPosition,
                rotation = child.transform.rotation,
            };
            
            itemDatas.Add(smallItemData);
        }

        return itemDatas;
    }

    private static List<LargeItemData> SaveLargeItemsData(Transform parent)
    {
        List<LargeItemData> itemDatas = new List<LargeItemData>();

        foreach (Transform child in parent)
        {
            LargeItemData smallItemData = new LargeItemData {
                itemPrefab = child.GetComponent<LargeItemBehaviour>().item.itemPrefab,
                position = child.transform.localPosition,
                rotation = child.transform.rotation,
            };
            itemDatas.Add(smallItemData);
        }

        return itemDatas;
    }

    private static List<LargeContainerData> SaveLargeContainersData(Transform parent)
    {
        List<LargeContainerData> itemDatas = new List<LargeContainerData>();

        foreach (Transform child in parent)
        {
            LargeContainerData smallItemData = new LargeContainerData {
                itemPrefab = child.GetComponent<LargeItemBehaviour>().item.itemPrefab,
                position = child.transform.localPosition,
                rotation = child.transform.rotation,
                inventoryData = SaveInventoryData(child.GetComponent<InventoryConteinerBehaviour>())
            };
            itemDatas.Add(smallItemData);
        }

        return itemDatas;
    }
    #endregion

}

[System.Serializable]
public class SaveGameData
{
    public PlayerData playerData;
    public WorldItemsData worldItemsData;
    public CarData carData;
}
[System.Serializable]
public class ExpeditionData
{
    public PlayerData playerData;
    public CarData carData;
}
[System.Serializable]
public class PlayerData
{
    public Vector3 position;
    public Vector2 cameraRotation;
    public InventoryData inventoryData;
    public List<InventoryData> slotsData;
}
[System.Serializable]
public class WorldItemsData
{
    public List<SmallItemData> SmallItemsData;
    public List<LargeItemData> LargeItemsData;
    public List<LargeContainerData> LargeContainerData;
}
[System.Serializable]
public class CarData
{
    public Vector3 position;
    public Quaternion rotation;
    public WorldItemsData carItemsData;
    public bool playerInCar;
}
[System.Serializable]
public class InventoryData
{
    public int invWidth;
    public int invHeight;
    public List<InventoryItemData> items;
}
[System.Serializable]
public class InventoryItemData
{
    public string itemType;
    public ItemSO item;
    public int x, y;
}
[System.Serializable]
public class SmallItemData
{
    public Vector3 position;
    public Quaternion rotation;
    public ItemSO item;
}
[System.Serializable]
public class LargeItemData
{
    public Vector3 position;
    public Quaternion rotation;
    public GameObject itemPrefab;
}
[System.Serializable]
public class LargeContainerData
{
    public Vector3 position;
    public Quaternion rotation;
    public GameObject itemPrefab;
    public InventoryData inventoryData;
}