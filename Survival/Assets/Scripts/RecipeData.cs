using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Recipes/New Recipte")]
public class RecipeData : ScriptableObject
{
    public ItemData craftableItem;
    public ItemInInventory[] requireItems;
}
