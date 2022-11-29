using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;

public class ItemActionsSystems : MonoBehaviour
{
    [Header("OTHER REFERENCES")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Equipement equipement;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private BuildSystem buildSystem;

    [Header("ACTION SYSTEM VARIABLES")]

    [SerializeField]
    private Transform dropPoint;

    [SerializeField]
    private GameObject useItemButton;

    [SerializeField]
    private GameObject equipItemButton;

    [SerializeField]
    private GameObject dropItemButton;

    [SerializeField]
    private GameObject PlaceItemButton;

    [SerializeField]
    private LayerMask layer;

    [HideInInspector]
    public ItemData itemCurrentlySelected;

    public GameObject actionPanel;

    private void Update()
    {
        Debug.DrawRay(player.position + new Vector3(0, 2, 0), -transform.up * 2, Color.red);
    }

    public void OpenActionPanel(ItemData item, Vector3 slotPosition)
    {
        itemCurrentlySelected = item;
        if (item == null)
        {
            actionPanel.SetActive(false);
            return;
        }

        switch (item.itemType)
        {
            case ItemType.Ressource:
                useItemButton.SetActive(false);
                equipItemButton.SetActive(false);
                PlaceItemButton.SetActive(false);
                break;
            case ItemType.Equipement:
                useItemButton.SetActive(false);
                equipItemButton.SetActive(true);
                PlaceItemButton.SetActive(false);
                break;
            case ItemType.Consumable:
                useItemButton.SetActive(true);
                equipItemButton.SetActive(false);
                PlaceItemButton.SetActive(false);
                break;
            case ItemType.Placable:
                useItemButton.SetActive(false);
                equipItemButton.SetActive(false);
                PlaceItemButton.SetActive(true);
                break;


        }

        actionPanel.transform.position = slotPosition;
        actionPanel.SetActive(true);
    }
    public void CloseActionPanel()
    {
        actionPanel.SetActive(false);
        itemCurrentlySelected = null;
    }

    public void UseActionButton()
    {

        if (itemCurrentlySelected.giveOtherItemOnUse)
        {
            for (int i = 0; i < itemCurrentlySelected.objectsGivenByUse.Length; i++)
            {
                if (Random.Range(1, 101) <= itemCurrentlySelected.objectsGivenByUse[i].lootChance)
                {
                    if (itemCurrentlySelected.objectsGivenByUse[i].conditionNeeded)
                    {
                        switch (itemCurrentlySelected.objectsGivenByUse[i].useConditions)
                        {
                            case UseConditions.IsWater:
                                RaycastHit hit;
                                if (Physics.Raycast(player.position + new Vector3(0,2,0), -transform.up, out hit, 2, layer))
                                {
                                    Inventory.Instance.AddItem(itemCurrentlySelected.objectsGivenByUse[i].itemData);
                                    playerStats.ConsumeItem(itemCurrentlySelected.healthEffect, itemCurrentlySelected.hungerEffect, itemCurrentlySelected.thirstEffect);
                                    Inventory.Instance.RemoveItem(itemCurrentlySelected);
                                    CloseActionPanel();
                                }
                                return;
                        }
                    }
                    else
                    {
                        Inventory.Instance.AddItem(itemCurrentlySelected.objectsGivenByUse[i].itemData);
                    }

                }
            }
        }
        playerStats.ConsumeItem(itemCurrentlySelected.healthEffect, itemCurrentlySelected.hungerEffect, itemCurrentlySelected.thirstEffect);
        Inventory.Instance.RemoveItem(itemCurrentlySelected);
        CloseActionPanel();
    }

    public void EquipActionButton()
    {
        equipement.EquipAction();
    }

    public void DropActionButton()
    {
        GameObject instanciatedItem = Instantiate(itemCurrentlySelected.prefab);
        instanciatedItem.transform.position = dropPoint.position;
        Inventory.Instance.RemoveItem(itemCurrentlySelected);
        Inventory.Instance.RefreshContent();
        CloseActionPanel();
    }

    public void PlaceActionButton()
    {
        Inventory.Instance.RemoveItem(itemCurrentlySelected);
        buildSystem.ChangeStructureItem(itemCurrentlySelected);
        CloseActionPanel();
        Inventory.Instance.CloseInventory();
    }


}
