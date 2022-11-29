using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Recipe : MonoBehaviour
{
    private RecipeData currentRecipe;

    [SerializeField]
    private Image craftableItemImage;

    [SerializeField]
    private GameObject elementRequirePrefab;

    [SerializeField]
    private Transform elementsRequiredParent;

    [SerializeField]
    private Button craftButton;

    [SerializeField]
    private Sprite canCraftIcon;

    [SerializeField]
    private Sprite cantCraftIcon;

    public void Configure(RecipeData recipe)
    {
        currentRecipe = recipe;

        craftableItemImage.sprite = recipe.craftableItem.visual;
        //permet l'afichage du tooltip présent dans slot quand on lui passe un item ici c l'item craft
        craftableItemImage.transform.parent.GetComponent<Slot>().item = recipe.craftableItem;

        bool canCraft = true;
        for (int i = 0; i < recipe.requireItems.Length; i++)
        {
            //récup tt les éléments  nécessaires au craft de l'item
            GameObject requiredItemGO = Instantiate(elementRequirePrefab, elementsRequiredParent);//requiredItemGO pour GameObject
            Image requiredItemGOImage = requiredItemGO.transform.GetChild(0).GetComponent<Image>();
            ItemData requiredItem = recipe.requireItems[i].itemData;

            //permet l'afichage du tooltip présent dans slot quand on lui passe un item ici c les items du craft
            requiredItemGO.GetComponent<Slot>().item = requiredItem;

            ItemInInventory[] itemInInventory = Inventory.Instance.GetContent().Where(elem => elem.itemData == requiredItem).ToArray();

            int totalRequiredItemQuantityInInventory = 0;
            for (int y = 0; y < itemInInventory.Length; y++)
            {
                totalRequiredItemQuantityInInventory += itemInInventory[y].count;
            }

            if (totalRequiredItemQuantityInInventory >= recipe.requireItems[i].count)
            {
                requiredItemGOImage.color = new Color(requiredItemGOImage.color.r, requiredItemGOImage.color.g, requiredItemGOImage.color.b, 1f);
            }else
            {
                requiredItemGOImage.color = new Color(requiredItemGOImage.color.r, requiredItemGOImage.color.g, requiredItemGOImage.color.b, 0.20f);

                canCraft = false;
            }

            requiredItemGO.transform.GetChild(0).GetComponent<Image>().sprite = recipe.requireItems[i].itemData.visual;
            if (recipe.requireItems[i].itemData.stackable)
            {
                requiredItemGO.transform.GetChild(1).GetComponent<Text>().text = recipe.requireItems[i].count.ToString();
            }

        }

        //gestion de l'affichage du boutton de craft

        craftButton.image.sprite = canCraft ? canCraftIcon : cantCraftIcon;
        craftButton.enabled = canCraft;

        ResizeElementsRequiredParent();
    }

    private void ResizeElementsRequiredParent()
    {
        Canvas.ForceUpdateCanvases();
        elementsRequiredParent.GetComponent<ContentSizeFitter>().enabled = false;
        elementsRequiredParent.GetComponent<ContentSizeFitter>().enabled = true;
    }

    public void CraftItem()
    {
        if (!Inventory.Instance.IsFull())
        {
            for (int i = 0; i < currentRecipe.requireItems.Length; i++)
            {
                for (int y = 0; y < currentRecipe.requireItems[i].count; y++)
                {
                    Inventory.Instance.RemoveItem(currentRecipe.requireItems[i].itemData);

                }
            }

            Inventory.Instance.AddItem(currentRecipe.craftableItem);
        }
    }

}
