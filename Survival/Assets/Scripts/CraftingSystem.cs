using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField]
    private RecipeData[] availableRecipes;

    [SerializeField]
    private GameObject recipeUiPrefab;

    [SerializeField]
    private Transform recipesParent;

    [SerializeField]
    private KeyCode openCraftPanelInput;

    [SerializeField]
    private GameObject craftingPanel;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplayedRecipes();
    }

    private void Update()
    {
        if (Input.GetKeyUp(openCraftPanelInput))
        {
            if (craftingPanel.activeSelf)
            {
                DisableCraftingPanel();
            }else
            {
                EnableCraftingPanel();
            }

        }
    }

    public void EnableCraftingPanel()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        craftingPanel.SetActive(true);
        UpdateDisplayedRecipes();
        TooltipSystem.instance.Hide();
    }

    public void DisableCraftingPanel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        craftingPanel.SetActive(false);
        UpdateDisplayedRecipes();
        TooltipSystem.instance.Hide();
    }

    public void UpdateDisplayedRecipes()
    {
        foreach (Transform child in recipesParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < availableRecipes.Length; i++)
        {
           GameObject recipe = Instantiate(recipeUiPrefab, recipesParent);
            recipe.GetComponent<Recipe>().Configure(availableRecipes[i]);
        }
    }


}
