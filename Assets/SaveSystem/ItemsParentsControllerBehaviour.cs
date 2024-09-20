using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsParentsControllerBehaviour : MonoBehaviour
{
    public static ItemsParentsControllerBehaviour Instance;

    [SerializeField] public Transform smallItemsParent;
    [SerializeField] public Transform largeItemsParent;
    [SerializeField] public Transform largeContainerParent;
    [SerializeField] public Transform staticContainerParent;

    private void Start()
    {
        Instance = this;
    }

}
