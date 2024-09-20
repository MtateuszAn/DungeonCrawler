
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystemBehaviour : MonoBehaviour
{
    [SerializeField] string saveFileFolder = "f1";
    public static SaveSystemBehaviour Instance { get; private set; }

    private static string saveFilePath;
    private static string saveExpeditionFilePath;
    private static string saveOnlyHomeFilePath;
    private void Awake()
    {
        // Implementacja wzorca Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcjonalnie, aby singleton przetrwa³ przez ró¿ne sceny
        }
        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileFolder + "/savefile.json");
        saveExpeditionFilePath = Path.Combine(Application.persistentDataPath, saveFileFolder + "/Expeditionsavefile.json");
        saveOnlyHomeFilePath = Path.Combine(Application.persistentDataPath, saveFileFolder + "/OnlyHomesavefile.json");
        Debug.Log(saveFilePath);
    }
    #region MainSaving
    public static void Save()
    {
        // Tworzenie instancji SaveData z danymi, które chcesz zapisaæ
        SaveGameData data = SaveSystem.SaveGame();

        // Serializacja danych do formatu JSON
        string json = JsonUtility.ToJson(data, true);

        // Zapisanie danych do pliku
        File.WriteAllText(saveFilePath, json);

    }
    public static void SaveBeforeExpedition()
    {
        WorldItemsData HomeData = SaveSystem.SaveHome();
        ExpeditionData expeditionData = SaveSystem.SaveExpedition();
        // Serializacja danych do formatu JSON
        string jsonExpe = JsonUtility.ToJson(expeditionData, true);
        string jsonHome = JsonUtility.ToJson(HomeData, true);

        // Zapisanie danych do pliku
        File.WriteAllText(saveExpeditionFilePath, jsonExpe);
        File.WriteAllText(saveOnlyHomeFilePath, jsonHome);
    }
    public static void SaveAfterExpedition()
    {
        ExpeditionData expeditionData = SaveSystem.SaveExpedition();

        // Serializacja danych do formatu JSON
        string json = JsonUtility.ToJson(expeditionData, true);

        // Zapisanie danych do pliku
        File.WriteAllText(saveExpeditionFilePath, json);

    }
    #endregion
    #region MainLoading
    public static void Load()
    {
        if (File.Exists(saveFilePath))
        {
            // Odczyt danych z pliku
            string json = File.ReadAllText(saveFilePath);

            // Deserializacja danych z formatu JSON
            SaveGameData data = JsonUtility.FromJson<SaveGameData>(json);

            // £adowanie sceny asynchronicznie
            SceneManager.LoadSceneAsync(1).completed += (AsyncOperation op) =>
            {
                Instance.StartCoroutine(LoadGame(data));
            };
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }

    internal void LoadNewExpedition()
    {
        SaveBeforeExpedition();
        if (File.Exists(saveExpeditionFilePath))
        {
            // Odczyt danych z pliku
            string json = File.ReadAllText(saveExpeditionFilePath);

            // Deserializacja danych z formatu JSON
            ExpeditionData data = JsonUtility.FromJson<ExpeditionData>(json);

            // £adowanie sceny asynchronicznie
            SceneManager.LoadSceneAsync(2).completed += (AsyncOperation op) =>
            {
                Instance.StartCoroutine(LoadOnStartNewExpedition(data));
            };
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }


    internal void LoadHomeFromExpedition()
    {
        SaveAfterExpedition();
        if (File.Exists(saveExpeditionFilePath) && File.Exists(saveOnlyHomeFilePath))
        {
            // Odczyt danych z pliku
            string json = File.ReadAllText(saveExpeditionFilePath);

            // Deserializacja danych z formatu JSON
            ExpeditionData dataExpedition = JsonUtility.FromJson<ExpeditionData>(json);

            json = File.ReadAllText(saveOnlyHomeFilePath);

            WorldItemsData dataHome = JsonUtility.FromJson<WorldItemsData>(json);
            // £adowanie sceny asynchronicznie
            SceneManager.LoadSceneAsync(1).completed += (AsyncOperation op) =>
            {
                Instance.StartCoroutine(LoadHomeAfterExpedition(dataExpedition, dataHome));
            };
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }
    #endregion
    #region LoadCoroutine
    private static IEnumerator LoadGame(SaveGameData data)
    {
        yield return new WaitForEndOfFrame();

        while (PlayerStateManager.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        Instance.LoadPlayer(data.playerData, true);
        Instance.LoadWorldItems(data.worldItemsData);
        Instance.LoadCar(data.carData, true);
        Debug.Log("Game loaded!");
    }

    private static IEnumerator LoadOnStartNewExpedition(ExpeditionData dataExpedition)
    {
        yield return new WaitForEndOfFrame();

        while (PlayerStateManager.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }
        Instance.LoadPlayer(dataExpedition.playerData, false);
        Instance.LoadCar(dataExpedition.carData, false);
        Debug.Log("Game loaded!");

    }
    private static IEnumerator LoadHomeAfterExpedition(ExpeditionData dataExpedition, WorldItemsData dataHome)
    {
        yield return new WaitForEndOfFrame();

        while (PlayerStateManager.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        Instance.LoadPlayer(dataExpedition.playerData, false);
        Instance.LoadWorldItems(dataHome);
        Instance.LoadCar(dataExpedition.carData, false);
        Debug.Log("Game loaded!");
    }
    #endregion
    #region LoadMethods
    private void LoadCar(CarData data, bool loadTransform)
    {
        if (loadTransform)
        {
            CarBehaviour.Instance.transform.position = data.position;
            CarBehaviour.Instance.transform.rotation = data.rotation;
        }
        LoadCarItems(data.carItemsData);
        if (data.playerInCar)
        {
            PlayerStateManager.Instance.GetInCar(CarBehaviour.Instance.mainSeat);
        }
    }

    private void LoadPlayer(PlayerData data, bool loadTransform)
    {
        if (loadTransform)
        {
            PlayerStateManager.Instance.transform.position = data.position;
            PlayerStateManager.Instance.cameraM.look = data.cameraRotation;
        }
        LoadInventory(data.inventoryData, PlayerStateManager.Instance.GetComponent<InventoryBehaviour>());
        for(int i=0; i< data.slotsData.Count; i++)
        {
            LoadInventory(data.slotsData[i], PlayerStateManager.Instance.beltManager.inventoryBeltSlots[i]);
        }
        
    }

    private void LoadInventory(InventoryData data,InventoryBehaviour inventory)
    {
        inventory.invHeight=data.invHeight;
        inventory.invWidth=data.invWidth;
        inventory.CreateSlots();
        foreach (InventoryItemData item in data.items) {
            if (!inventory.LoadItems(item))
                Debug.Log("Failed to create Item");
        }
    }

    private void LoadWorldItems(WorldItemsData data)
    {
        LoadSmallItems(data, ItemsParentsControllerBehaviour.Instance.smallItemsParent);
        LoadLargeItems(data, ItemsParentsControllerBehaviour.Instance.largeItemsParent);
        LoadLargeContainerItems(data, ItemsParentsControllerBehaviour.Instance.largeContainerParent);
    }
    private void LoadCarItems(WorldItemsData data)
    {
        LoadSmallItems(data, CarBehaviour.Instance.smallItemsParent);
        LoadLargeItems(data, CarBehaviour.Instance.largeItemsParent);
        LoadLargeContainerItems(data, CarBehaviour.Instance.largeContainerParent);
    }

    private void LoadSmallItems(WorldItemsData data, Transform parent)
    {
        foreach (SmallItemData item in data.SmallItemsData)
        {
            GameObject newItem = Instantiate(item.item.itemPrefab);
            newItem.transform.parent = parent;
            newItem.transform.localPosition = item.position;
            newItem.transform.rotation = item.rotation;
        }
    }
    private void LoadLargeItems(WorldItemsData data, Transform parent)
    {
        foreach (LargeItemData item in data.LargeItemsData)
        {
            GameObject newItem = Instantiate(item.itemPrefab);
            newItem.transform.parent = parent;
            newItem.transform.localPosition = item.position;
            newItem.transform.rotation = item.rotation;
        }
    }
    private void LoadLargeContainerItems(WorldItemsData data, Transform parent)
    {
        foreach (LargeContainerData item in data.LargeContainerData)
        {
            GameObject newItem = Instantiate(item.itemPrefab);
            newItem.transform.parent = parent;
            newItem.transform.localPosition = item.position;
            newItem.transform.rotation = item.rotation;
            LoadInventory(item.inventoryData,newItem.GetComponent<InventoryConteinerBehaviour>());
        }
    }

    
    #endregion
}

