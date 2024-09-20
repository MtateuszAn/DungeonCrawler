using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void NewGame()
    {
        SceneManager.LoadSceneAsync(3);
    }
    public void LoadGame()
    {
        SaveSystemBehaviour.Load();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}