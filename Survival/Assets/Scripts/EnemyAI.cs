using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    private Transform player;
    private PlayerStats playerStats;

    [Header("References")]
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject healthBarPrefab;
    private GameObject healthBar = null;
    [SerializeField]
    private Vector3 healthBarOffset;

    [SerializeField]
    private GameObject lootUIPrefab;
    [HideInInspector]
    public GameObject lootUI = null;

    [SerializeField]
    private GameObject slotPrefab;

    [SerializeField]
    private GameObject camMovement;

    [Header("Stats")]
    [SerializeField]
    private float detectionRaduis;

    [SerializeField]
    private float attackRaduis;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float ChaseSpeed;

    [SerializeField]
    private float attackDelay;

    [SerializeField]
    private float attackDamage;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float currentHealth;

    [SerializeField]
    private DropSystem[] drops;

    [Header("Wandering parameters")]
    [SerializeField]
    private float wanderingWaitTimeMin;

    [SerializeField]
    private float wanderingWaitTimeMax;

    [SerializeField]
    private float wanderingDistanceMin;

    [SerializeField]
    private float wanderingDistanceMax;

    private bool hasDestination;
    private bool isAttacking;
    public bool isDead;

    private void Awake()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerTransform;
        playerStats = playerTransform.GetComponent<PlayerStats>();

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBar != null)
        {
            Vector3 wantedPos = Camera.main.WorldToScreenPoint(this.transform.position + healthBarOffset);
            healthBar.transform.position = wantedPos;
            healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBarPrefab.GetComponent<RectTransform>().sizeDelta.x / Vector3.Distance(transform.position, player.position)*5, healthBarPrefab.GetComponent<RectTransform>().sizeDelta.y / Vector3.Distance(transform.position, player.position)*5);
            healthBar.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(healthBarPrefab.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x / Vector3.Distance(transform.position, player.position)*5, healthBarPrefab.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y / Vector3.Distance(transform.position, player.position)*5);
            healthBar.transform.GetChild(0).GetComponent<Image>().fillAmount = currentHealth / maxHealth;
        }
        if (lootUI != null)
        {
            Vector3 wantedPos = Camera.main.WorldToScreenPoint(this.transform.position);
            lootUI.transform.position = wantedPos;
            if (Vector3.Distance(this.transform.position, player.position) > 10f && lootUI.activeSelf)
            {
                camMovement.SetActive(false);
                lootUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                TooltipSystem.instance.Hide();


            }
            else
            {
                lootUI.SetActive(true);
                if (camMovement.activeSelf)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        camMovement.SetActive(false);
                        Cursor.lockState = CursorLockMode.Locked;
                        TooltipSystem.instance.Hide();
                    }
                    if (lootUI.transform.childCount == 0)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        camMovement.SetActive(false);
                        TooltipSystem.instance.Hide();
                    }
                }
            }
            


        }

        if (isDead)
        {
            return;
        }
        if (Vector3.Distance(player.position, transform.position) < detectionRaduis && !playerStats.isDead)
        {
            agent.speed = ChaseSpeed;
            if (healthBar == null)
            {
                SummonHealthBar();
            }

            if (Vector3.Distance(player.position, transform.position) < attackRaduis)
                {
                    Quaternion rot = Quaternion.LookRotation(player.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
                if (isAttacking)
                {
                    return;
                }
                    StartCoroutine(AttackPlayer());
                }
                else
                {
                if (isAttacking)
                {
                    return;
                }

                agent.SetDestination(player.position);
                }

           
        } 
        else
        {
            if (healthBar != null)
            {
                Destroy(healthBar);
                healthBar = null;
            }
            agent.speed = walkSpeed;
            if (agent.remainingDistance < 0.75f && !hasDestination)
            {
                StartCoroutine(GetNewDestination());
            }
        }
        animator.SetFloat("Speed",agent.velocity.magnitude);
    }

    private void SummonHealthBar()
    {
        healthBar = Instantiate(healthBarPrefab,canvas.transform);
    }

    public void TakeDammage(float dammage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= dammage;
        if (currentHealth < 0)
        {
            StartCoroutine(Die());
        }else
        {
            animator.SetTrigger("GetHit");
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        agent.enabled = false;
        Destroy(healthBar);
        healthBar = null;
        if (drops.Length == 0)
        {
            yield break;
        }
        lootUI = Instantiate(lootUIPrefab, canvas.transform);
        foreach (var drop in drops)
        {
            if (Random.Range(1, 101) <= drop.dropChance)
            {
                GameObject slot = Instantiate(slotPrefab, lootUI.transform);
                slot.transform.GetChild(0).GetComponent<Image>().sprite = drop.item.visual;
                slot.GetComponent<Slot>().item = drop.item;
            }

        }
    }

    IEnumerator GetNewDestination()
    {
        hasDestination = true;
        yield return new WaitForSeconds(Random.Range(wanderingWaitTimeMin, wanderingWaitTimeMax));

        Vector3 nextDestination = transform.position;
        nextDestination += Random.Range(wanderingDistanceMin, wanderingDistanceMax) * new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(nextDestination, out hit, wanderingDistanceMax, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        hasDestination = false;
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        agent.isStopped = true;

        animator.SetTrigger("Attack");
        playerStats.TakeDamage(attackDamage);
        yield return new WaitForSeconds(attackDelay);
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
        isAttacking =false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRaduis);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRaduis);
    }
}

[System.Serializable]
public class DropSystem
{
    [SerializeField]
    public ItemData item;
    [SerializeField, Range(0f, 100f)]
    public float dropChance;
}