using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/New item")]
public class ItemData : ScriptableObject
{

    [Header("DATA")]
    public string name;
    public string description;
    public Sprite visual;
    public GameObject prefab;
    public bool stackable;
    public int maxStack;
    public bool giveOtherItemOnUse;
    [SerializeField]
    public ObjectsGivenByUse[] objectsGivenByUse;


    [Header("TYPES")]
    public ItemType itemType;
    public EquipementType equipementType;

    [Header("EFFECTS")]
    public float healthEffect;
    public float hungerEffect;
    public float thirstEffect;


    [Header("Armor Stats")]
    public float armorPoints;

    [Header("Weapon Stats")]
    public float attackDamage;
}

public enum ItemType
{
    Ressource,
    Equipement,
    Consumable,
    Placable
}

public enum EquipementType
{
    Head,
    Chest,
    Hands,
    Legs,
    Feet,
    Weapon

}

public enum UseConditions
{
    IsWater
}

[System.Serializable]
public class ObjectsGivenByUse
{
    public ItemData itemData;
    [Range(0, 100)]
    public int lootChance;

    public bool conditionNeeded;
    public UseConditions useConditions;

}

