using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform playerCamera;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private Animator Animator;


    [SerializeField]
    private InteractBehaviour interactBehaviour;

    [SerializeField]
    private Equipement equipement;

    [SerializeField]
    private float attackRange;

    private bool isAttacking;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Vector3 attackOffset;
    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(playerCamera.position + attackOffset, playerCamera.forward * attackRange, Color.red);
        if (Input.GetMouseButtonDown(0) && CanAttack())
        {
            isAttacking = true;
            SendAttack();
            Animator.SetTrigger("Attack");
        }
    }

    void SendAttack()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.position + attackOffset, playerCamera.forward, out hit, attackRange,layerMask))
        {

            EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();
            enemy.TakeDammage(equipement.equipedWeaponItem.attackDamage);
        }
    }

    bool CanAttack()
    {
        return equipement.equipedWeaponItem != null && !isAttacking && !uiManager.atLeastOnePanelOpened && !interactBehaviour.isBusy;
    }

    public void AttackFinished()
    {
        isAttacking = false;
    }
}
