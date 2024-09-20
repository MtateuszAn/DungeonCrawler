using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UiPopUpManager : MonoBehaviour
{
    public static UiPopUpManager Instance;
    [SerializeField] GameObject popUpTemplate;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreatePopUp(string text)
    {
        GameObject instance = Instantiate(popUpTemplate,gameObject.transform);
        instance.GetComponent<TMP_Text>().text = text;
        StartCoroutine(PopUpCoreRutine(instance));
    }
    IEnumerator PopUpCoreRutine(GameObject instance) 
    {
        yield return new WaitForSeconds(3f);
        GameObject.Destroy(instance);
        yield return null; 
    }
}
