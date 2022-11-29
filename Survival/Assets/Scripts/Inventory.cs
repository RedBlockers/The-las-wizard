using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [Header("OTHER SCRIPTS REFERENCES")]

    [SerializeField]
    private Equipement equipement;

    [SerializeField]
    private ItemActionsSystems itemActionsSystems;

    [SerializeField]
    private CraftingSystem craftingSystem;

    [Header("INVENTORY SYSTEM VARIABLES")]

    [SerializeField]
    private GameObject craftingPanel;

    [SerializeField]
    private List<ItemInInventory> content = new List<ItemInInventory>();

    [SerializeField]
    GameObject inventoryPanel;

    [SerializeField]
    private Transform inventorySlotsParent;

    const int INVENTORY_SIZE = 24;



    public Sprite emptySlotVisual;
    public static Inventory Instance;
    private bool isOpen = false;


    public void Awake()
    {
        Instance = this;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        CloseInventory();
        RefreshContent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isOpen)
            {
                CloseInventory();
            }else
            {
                OpenInventory();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (craftingPanel.activeSelf)
            {
                craftingPanel.SetActive(false);
                TooltipSystem.instance.Hide();

            }
            else if (itemActionsSystems.actionPanel.activeSelf)
            {
                itemActionsSystems.CloseActionPanel();
            }
            else if (isOpen)
            {
                CloseInventory();
            }


        }
    }

    public void AddItem(ItemData item)
    {
        ItemInInventory[] itemInInventory = content.Where(elem => elem.itemData == item).ToArray();// récupere ds l'inventaire un item étant le mm que l'on éssaye d'ajouter

        bool itemAdded = false;

        if (itemInInventory.Length > 0 && item.stackable)
        {
            for (int i = 0; i < itemInInventory.Length; i++)
            {
                if (itemInInventory[i].count < item.maxStack)
                {
                    itemAdded = true;
                    itemInInventory[i].count++;
                    break;
                }
            }

            if (!itemAdded)
            {
                content.Add(
                    new ItemInInventory
                    {
                        itemData = item,
                        count = 1
                    });
            }
   
        }
        else
        {
            content.Add(
                new ItemInInventory
                {
                    itemData = item,
                    count = 1
                });
        }

        RefreshContent();
    }

    public void RemoveItem(ItemData item)
    {
        ItemInInventory itemInInventory = content.Where(elem => elem.itemData == item).LastOrDefault();// récupere ds l'inventaire un item étant le mm que l'on éssaye d'ajouter
        
        if (itemInInventory != null && itemInInventory.count > 1)
        {
            itemInInventory.count --;
        }else
        {
            content.Remove(itemInInventory);
        }

        RefreshContent();
    }

    public List<ItemInInventory> GetContent()
    {
        return content;
    }

    public void OpenInventory()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inventoryPanel.SetActive(true);
        isOpen = true;
    }

    public void CloseInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inventoryPanel.SetActive(false);
        itemActionsSystems.actionPanel.SetActive(false);
        TooltipSystem.instance.Hide();
        isOpen = false;
    }

    public void RefreshContent()
    {

        //vidage de tout les slots de l'inventaire
        for (int i = 0; i < inventorySlotsParent.childCount; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
            currentSlot.countText.enabled = false;
        }


        //Peuplement du visuel selon le contenue réel de l'inventaire
        for (int i = 0; i < content.Count; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = content[i].itemData;
            currentSlot.itemVisual.sprite = content[i].itemData.visual;

            if (currentSlot.item.stackable)
            {
                currentSlot.countText.enabled = true;
                currentSlot.countText.text = content[i].count.ToString();
            }
        }

        equipement.UpdateEquipementDesequipComponant();
        craftingSystem.UpdateDisplayedRecipes();
    }

    public bool IsFull()
    {
        return INVENTORY_SIZE == content.Count;
    }

}

[System.Serializable]
public class ItemInInventory
{
    public ItemData itemData;
    public int count;
}