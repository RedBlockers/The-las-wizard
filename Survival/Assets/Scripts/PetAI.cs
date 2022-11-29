using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class PetAI : MonoBehaviour
{

    private Transform player;
    private PlayerStats playerStats;

    [Header("References")]
    [SerializeField]
    private NavMeshAgent agent;

    [Header("Stats")]
    [SerializeField]
    private float detectionRaduis;

    [SerializeField]
    private float stopFollowRadius;

    [SerializeField]
    private float walkRadius;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float RunSpeed;

    [SerializeField]
    private float rotationSpeed;

    [Header("Wandering parameters")]
    [SerializeField]
    private float wanderingWaitTimeMin;

    [SerializeField]
    private float wanderingWaitTimeMax;

    [SerializeField]
    private float wanderingDistanceMin;

    [SerializeField]
    private float wanderingDistanceMax;

    private bool isSitted = false;

    private void Awake()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerTransform;
        playerStats = playerTransform.GetComponent<PlayerStats>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) < detectionRaduis && Vector3.Distance(player.position, transform.position) > stopFollowRadius && !isSitted)
        {
            if (Vector3.Distance(player.position, transform.position) > walkRadius)
            {
                agent.speed = RunSpeed;
            }else
            {
                agent.speed = walkSpeed;
            }

            agent.SetDestination(player.position + new Vector3(player.right.x,0,player.right.z));

        }
        else
        {
            agent.speed = walkSpeed;

            if (!isSitted && agent.remainingDistance < 0.1f)
            {
                isSitted = true;
                animator.SetTrigger("Sit");
            }
            else if (Vector3.Distance(player.position, transform.position) > stopFollowRadius && isSitted )
            {
                isSitted = false;
                animator.SetTrigger("StandUp");
            }
        }
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRaduis);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopFollowRadius);
    }
}
