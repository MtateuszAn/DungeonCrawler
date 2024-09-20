using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuBehaviour : MonoBehaviour
{
    public void Save()
    {
        SaveSystemBehaviour.Save();
    }
    public void Load()
    {
        SaveSystemBehaviour.Load();
    }
}
