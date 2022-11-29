using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractBehaviour : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField]
    private MoveBehaviour playerMoveBehaviour;

    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private Equipement equipement;

    [SerializeField]
    private EquipementLibrary equipementLibrary;

    [Header("TOOLS VISUALS")]
    [SerializeField]
    private GameObject pickaxeVisual;

    [SerializeField]
    private GameObject AxeVisual;

    private Item currentItem;
    private Hardvestable currentHardvestable;
    private Tool currentTool;
    private float YSpawnItemOffset = 0.5f;
    private List<Hardvestable> harvestableToDestroy;
    [HideInInspector]
    public bool isBusy = false;


    public void DoPickup(Item item)
    {
        if (isBusy)
        {
            return;
        }


        if (inventory.IsFull())
        {
            Debug.Log("Inventory full");
            return;
        }

        isBusy = true;

        currentItem = item;

        playerAnimator.SetTrigger("Pickup"); //joue l'animation du joueur
        playerMoveBehaviour.canMove = false;
    }

    public void DoHardvest(Hardvestable hardvestable)
    {

        if (isBusy)
        {
            return;
        }

        isBusy = true;

        currentTool = hardvestable.tool;
        EnableToolGameObjectFromEnum(currentTool);

        currentHardvestable = hardvestable;//variable temporaire afin de definir quel est l'élément de récolte actuel
        playerAnimator.SetTrigger("Hardvest");
        playerMoveBehaviour.canMove = false;

    }

    //coroutine appelée depuis l'animation "mining loop"
    IEnumerator BreakHardvestable()
    {
        if (harvestableToDestroy == null)
        {
            harvestableToDestroy = new List<Hardvestable>();
        }
        // permet de désactiver l'intération avec un hardvestable déjà traiter
        currentHardvestable.gameObject.layer = LayerMask.NameToLayer("Unhardvestable");

        harvestableToDestroy.Add(currentHardvestable);
        if (currentHardvestable.disableKinematicOnHardvest)
        {
            Rigidbody rigidbody = currentHardvestable.gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.AddForce(transform.forward*1500, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(currentHardvestable.destroyDelay);

        for (int i = 0; i < harvestableToDestroy[0].hardvestableItems.Length; i++)
        {
            Ressources ressources = harvestableToDestroy[0].hardvestableItems[i];

            if (Random.Range(1, 101) <= ressources.dropChance)
            {
                GameObject instantiatedRessource = Instantiate(ressources.itemData.prefab);
                instantiatedRessource.transform.position = harvestableToDestroy[0].transform.position + new Vector3(Random.Range((float)-1f, (float)1f), YSpawnItemOffset, Random.Range((float)-1f, (float)1f));// le vector 3 définit une position aléatoir aux pierres qui spawn a partir du rocher
            }
        }

        Destroy(harvestableToDestroy[0].gameObject);
        harvestableToDestroy.Remove(harvestableToDestroy[0]);
    }

    public void AddItemToInventory()
    {
        inventory.AddItem(currentItem.itemData); //ajoute l'item au joueur
        Destroy(currentItem.gameObject);
    }

    public void ReEnablePlayerMovement()
    {
        EnableToolGameObjectFromEnum(currentTool, false);
        playerMoveBehaviour.canMove = true;
        isBusy = false;
    }

    private void EnableToolGameObjectFromEnum(Tool tooltype, bool enabled = true)
    {
        if (equipement.equipedWeaponItem)
        {
            EquipementLibraryItem equipementLibraryItem = equipementLibrary.content.Where(elem => elem.itemData == equipement.equipedWeaponItem).First();  //recup couple item/visuel

            if (equipementLibraryItem != null)
            {
                for (int i = 0; i < equipementLibraryItem.elementToDisable.Length; i++)
                {
                    equipementLibraryItem.elementToDisable[i].SetActive(enabled);
                }

                equipementLibraryItem.itemPrefab.SetActive(!enabled);
            }
        }
        

        switch (tooltype)
        {
            case Tool.Pickaxe:
                pickaxeVisual.SetActive(enabled);
                break;
            case Tool.Axe:
                AxeVisual.SetActive(enabled);
                break;
        }
    }


}
