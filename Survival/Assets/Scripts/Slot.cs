using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public ItemData item;
    public Image itemVisual;
    public Text countText;

    [SerializeField]
    private ItemActionsSystems itemActionsSystems;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            TooltipSystem.instance.Show(item.description, item.name);

        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.instance.Hide();
    }

    public void ClickOnSlot()
    {
        itemActionsSystems.OpenActionPanel(item,transform.position);
    }

    public void GiveItemToPlayer()
    {
        if (Inventory.Instance.IsFull())
        {
            return;
        }
        Inventory.Instance.AddItem(item);
        Destroy(this.transform.gameObject);
        TooltipSystem.instance.Hide();

    }

}
