using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipementLibrary : MonoBehaviour
{
    public List<EquipementLibraryItem> content = new List<EquipementLibraryItem>();
}

[System.Serializable]
public class EquipementLibraryItem
{
    public ItemData itemData;
    public GameObject itemPrefab;
    public GameObject[] elementToDisable;
}