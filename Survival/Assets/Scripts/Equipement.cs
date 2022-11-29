using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Equipement : MonoBehaviour
{
    [Header("OTHER SCRIPTS REFERENCES")]

    [SerializeField]
    private ItemActionsSystems itemActionsSystems;

    [SerializeField]
    private PlayerStats playerStats;

    [Header("EQUIPEMENT SYSTEM VARIABLES")]

    [SerializeField]
    private EquipementLibrary equipementLibrary;

    //récup les slots d'images
    [SerializeField]
    private Image headSlotImage;

    [SerializeField]
    private Image chestSlotImage;

    [SerializeField]
    private Image handsSlotImage;

    [SerializeField]
    private Image legsSlotImage;

    [SerializeField]
    private Image feetSlotImage;

    [SerializeField]
    private Image weaponSlotImage;


    //récup les boutons de désequipements 
    [SerializeField]
    private Button headSlotButton;

    [SerializeField]
    private Button chestSlotButton;

    [SerializeField]
    private Button handsSlotButton;

    [SerializeField]
    private Button legsSlotButton;

    [SerializeField]
    private Button feetSlotButton;

    [SerializeField]
    private Button weaponSlotButton;

    //Garde une trace des équipements actuels
    private ItemData equipedHeadItem;
    private ItemData equipedChestItem;
    private ItemData equipedHandItem;
    private ItemData equipedLegsItem;
    private ItemData equipedFeetItem;
    [HideInInspector]
    public ItemData equipedWeaponItem;


    private void DisablePreviousEquipedEquipement(ItemData itemtoDisable)
    {
        if (itemtoDisable == null)
        {
            return;
        }
        EquipementLibraryItem equipementLibraryItem = equipementLibrary.content.Where(elem => elem.itemData == itemtoDisable).First();  //recup couple item/visuel

        if (equipementLibraryItem != null)
        {
            for (int i = 0; i < equipementLibraryItem.elementToDisable.Length; i++)//désactive les parties néssesaires 
            {
                equipementLibraryItem.elementToDisable[i].SetActive(true);
            }

            equipementLibraryItem.itemPrefab.SetActive(false);// active l'equipement
        }

        playerStats.currentArmor -= itemtoDisable.armorPoints;
        Inventory.Instance.AddItem(itemtoDisable);

    }

    public void DesequipEquipement(EquipementType equipementType)
    {
        if (Inventory.Instance.IsFull())
        {
            Debug.Log("inventaire plein impossible de déséquiper");
            return;
        }
        ItemData currentItem = null;

        switch (equipementType)
        {
            case EquipementType.Head:
                currentItem = equipedHeadItem;
                equipedHeadItem = null;
                headSlotImage.sprite = Inventory.Instance.emptySlotVisual;
                break;

            case EquipementType.Chest:
                currentItem = equipedChestItem;
                equipedChestItem = null;
                chestSlotImage.sprite = Inventory.Instance.emptySlotVisual;
                break;

            case EquipementType.Hands:
                currentItem = equipedHandItem;
                equipedHandItem = null;
                handsSlotImage.sprite = Inventory.Instance.emptySlotVisual;
                break;

            case EquipementType.Legs:
                currentItem = equipedLegsItem;
                equipedLegsItem = null;
                legsSlotImage.sprite = Inventory.Instance.emptySlotVisual;
                break;

            case EquipementType.Feet:
                currentItem = equipedFeetItem;
                equipedFeetItem = null;
                feetSlotImage.sprite = Inventory.Instance.emptySlotVisual;
                break;
            case EquipementType.Weapon:
                currentItem = equipedWeaponItem;
                equipedWeaponItem = null;
                weaponSlotImage.sprite = Inventory.Instance.emptySlotVisual;
                break;
        }

        EquipementLibraryItem equipementLibraryItem = equipementLibrary.content.Where(elem => elem.itemData == currentItem).First();  //recup couple item/visuel

        if (equipementLibraryItem != null)
        {
            for (int i = 0; i < equipementLibraryItem.elementToDisable.Length; i++)//désactive les parties néssesaires 
            {
                equipementLibraryItem.elementToDisable[i].SetActive(true);
            }

            equipementLibraryItem.itemPrefab.SetActive(false);// active l'equipement
        }

        playerStats.currentArmor -= currentItem.armorPoints;

        Inventory.Instance.AddItem(currentItem);
        Inventory.Instance.RefreshContent();
    }

    public void UpdateEquipementDesequipComponant()
    {
        headSlotButton.onClick.RemoveAllListeners();
        headSlotButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Head); });
        headSlotButton.GetComponent<Button>().enabled = equipedHeadItem;

        chestSlotButton.onClick.RemoveAllListeners();
        chestSlotButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Chest); });
        chestSlotButton.GetComponent<Button>().enabled = equipedChestItem;

        handsSlotButton.onClick.RemoveAllListeners();
        handsSlotButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Hands); });
        handsSlotButton.GetComponent<Button>().enabled = equipedHandItem;

        legsSlotButton.onClick.RemoveAllListeners();
        legsSlotButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Legs); });
        legsSlotButton.GetComponent<Button>().enabled = equipedLegsItem;

        feetSlotButton.onClick.RemoveAllListeners();
        feetSlotButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Feet); });
        feetSlotButton.GetComponent<Button>().enabled = equipedFeetItem;

        weaponSlotButton.onClick.RemoveAllListeners();
        weaponSlotButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Weapon); });
        weaponSlotButton.GetComponent<Button>().enabled = equipedWeaponItem;
    }

    public void EquipAction()
    {
        print("equip item :" + itemActionsSystems.itemCurrentlySelected.name);

        EquipementLibraryItem equipementLibraryItem = equipementLibrary.content.Where(elem => elem.itemData == itemActionsSystems.itemCurrentlySelected).First();  //recup couple item/visuel

        if (equipementLibraryItem != null)
        {


            switch (itemActionsSystems.itemCurrentlySelected.equipementType)// set le visual dans la partie équipement de l'inventaire
            {
                case EquipementType.Head:
                    DisablePreviousEquipedEquipement(equipedHeadItem);
                    headSlotImage.sprite = itemActionsSystems.itemCurrentlySelected.visual;
                    equipedHeadItem = itemActionsSystems.itemCurrentlySelected;
                    break;

                case EquipementType.Chest:
                    DisablePreviousEquipedEquipement(equipedChestItem);
                    chestSlotImage.sprite = itemActionsSystems.itemCurrentlySelected.visual;
                    equipedChestItem = itemActionsSystems.itemCurrentlySelected;
                    break;

                case EquipementType.Hands:
                    DisablePreviousEquipedEquipement(equipedHandItem);
                    handsSlotImage.sprite = itemActionsSystems.itemCurrentlySelected.visual;
                    equipedHandItem = itemActionsSystems.itemCurrentlySelected;
                    break;

                case EquipementType.Legs:
                    DisablePreviousEquipedEquipement(equipedLegsItem);
                    legsSlotImage.sprite = itemActionsSystems.itemCurrentlySelected.visual;
                    equipedLegsItem = itemActionsSystems.itemCurrentlySelected;
                    break;

                case EquipementType.Feet:
                    DisablePreviousEquipedEquipement(equipedFeetItem);
                    feetSlotImage.sprite = itemActionsSystems.itemCurrentlySelected.visual;
                    equipedFeetItem = itemActionsSystems.itemCurrentlySelected;
                    break;
                case EquipementType.Weapon:
                    DisablePreviousEquipedEquipement(equipedWeaponItem);
                    weaponSlotImage.sprite = itemActionsSystems.itemCurrentlySelected.visual;
                    equipedWeaponItem = itemActionsSystems.itemCurrentlySelected;
                    break;

            }

            for (int i = 0; i < equipementLibraryItem.elementToDisable.Length; i++)//désactive les parties néssesaires 
            {
                equipementLibraryItem.elementToDisable[i].SetActive(false);
            }

            equipementLibraryItem.itemPrefab.SetActive(true);// active l'equipement

            playerStats.currentArmor += itemActionsSystems.itemCurrentlySelected.armorPoints;

            Inventory.Instance.RemoveItem(itemActionsSystems.itemCurrentlySelected);
        }
        else
        {
            Debug.LogError("Equipement : " + itemActionsSystems.itemCurrentlySelected.name + "Does not exist in the item library");
        }

        itemActionsSystems.CloseActionPanel();
    }
}
