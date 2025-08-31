using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Base : MonoBehaviour
{
    // Reference Variables
    FirstPersonController player;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();
    }

    void Update()
    {
        agent.SetDestination(player.transform.position);
    }
}
