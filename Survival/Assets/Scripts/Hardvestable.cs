using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hardvestable : MonoBehaviour
{
    [SerializeField]
    public Ressources[] hardvestableItems;
    [Header("OPTIONS")]
    public Tool tool;
    public bool disableKinematicOnHardvest;
    public float destroyDelay;
}

[System.Serializable]
public class Ressources
{
    public ItemData itemData;

    [Range(0,100)]
    public int dropChance;


}

public enum Tool
{
    Pickaxe,
    Axe
}