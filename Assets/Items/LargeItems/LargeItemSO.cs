using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LargeItem", menuName = "LargeItem/Create New LargeItem")]
public class LargeItemSO : ScriptableObject
{
    [SerializeField] public GameObject itemPrefab;
}
