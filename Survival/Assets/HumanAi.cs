using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAi : MonoBehaviour
{
    [Header("Other Objects References")]
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private LightingManager lightingManager;


    [Header("Agent Settings")]
    [SerializeField]
    private float speed;


    [Header("NPC Settings")]
    [SerializeField]
    private string firstName;
    [SerializeField]
    private string lastName;
    [SerializeField]
    private Faction faction;
    [SerializeField]
    private Faction[] hostileFaction;
    [SerializeField]
    private bool canTakeDamage;
    [SerializeField]
    private float maxHeal;
    [SerializeField]
    private float currentHealth;
    [SerializeField]
    private Schedules[] schedules;


    private bool hasDestination = false;



    private void Update()
    {
        if (schedules != null && !hasDestination)
        {
            for (int i = 0; i < schedules.Length; i++)
            {
                if (schedules.Length > i + 1)
                {

                    if (lightingManager.TimeOfDay >= schedules[i].hour && lightingManager.TimeOfDay < schedules[i + 1].hour)
                    {
                        StartCoroutine(MoveToDestination(schedules[i]));
                        break;
                    }
                }else
                {
                    StartCoroutine(MoveToDestination(schedules[i]));
                    break;

                }
            }
        }
    }

    IEnumerator MoveToDestination(Schedules schedule)
    {
        if (Vector3.Distance(this.transform.position, schedule.destination.transform.position) < 0.3f && ! schedule.loop)
        {
            yield break;
        }
        hasDestination = true;
        foreach (var waypoint in schedule.waypoints)
        {
            while (Vector3.Distance(this.transform.position, waypoint.transform.position) > 0.3f)
            {
                agent.SetDestination(waypoint.transform.position);
                yield return new WaitForSeconds(0.5f);
            }
        }
        agent.SetDestination(schedule.destination.transform.position);
        while (Vector3.Distance(this.transform.position, schedule.destination.transform.position) > 0.3f)
        {
            yield return new WaitForSeconds(0.5f);
        }
        this.transform.rotation = schedule.destination.transform.rotation;
        hasDestination = false;
    }

}

public enum Faction
{
    Citizen,
    Villager,
    Guard,
    Hostile
}

[System.Serializable]
public class Schedules
{
    [Range(0f, 24f)]
    public float hour;
    public GameObject destination;
    public GameObject[] waypoints;
    public bool loop;
}