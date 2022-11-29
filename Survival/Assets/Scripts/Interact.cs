using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    [SerializeField]
    private float interactRange = 3.4f;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private GameObject InteractText;

    [SerializeField]
    private GameObject camMovement;

    public InteractBehaviour playerInteractBehaviour;

    [SerializeField]
    private Animator foxAnimator;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, interactRange, layerMask))
        {
            InteractText.SetActive(true);
            {
                if (hit.transform.CompareTag("Item"))
                {
                    InteractText.GetComponent<Text>().text = "Press E to pick up";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        playerInteractBehaviour.DoPickup(hit.transform.gameObject.GetComponent<Item>());
                    }
                }
                else if (hit.transform.CompareTag("Hardvestable"))
                {
                    InteractText.GetComponent<Text>().text = "Press E to mine";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        playerInteractBehaviour.DoHardvest(hit.transform.gameObject.GetComponent<Hardvestable>());
                    }
                }
                else if (hit.transform.CompareTag("Fox"))
                {
                    InteractText.GetComponent<Text>().text = "Press E to interact";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if ((Random.Range(1f, 2f) < 1.5))
                        {
                            foxAnimator.SetTrigger("YES");
                        }
                        else
                        {
                            foxAnimator.SetTrigger("NO");
                        }
                    }
                }
                else if (hit.transform.CompareTag("Bear"))
                {
                    if (hit.transform.gameObject.GetComponent<EnemyAI>().isDead && hit.transform.gameObject.GetComponent<EnemyAI>().lootUI.transform.childCount > 0)
                    {
                        InteractText.GetComponent<Text>().text = "Press E to loot";
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                            camMovement.SetActive(true);
                        }
                    }
                }
            }
        }
        else
        {
            InteractText.SetActive(false);
        }
    }
}
