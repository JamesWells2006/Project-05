using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Return }
    
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 8f;
    public float attackRange = 1.5f;
    public Transform[] patrolPoints;
    public float waitTime = 1f;
    public Light detectionLight;
    public Color normalColor = Color.yellow;
    public Color alertColor = Color.red;
    
    private NavMeshAgent agent;
    private Transform player;
    private EnemyState currentState;
    private int currentPatrolIndex = 0;
    private bool waiting = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        currentState = EnemyState.Patrol;
        agent.speed = patrolSpeed;
        
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
        
        if (detectionLight != null)
        {
            detectionLight.color = normalColor;
        }
    }
    
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                
                // Check if player is within detection range
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    currentState = EnemyState.Chase;
                    agent.speed = chaseSpeed;
                    
                    if (detectionLight != null)
                    {
                        detectionLight.color = alertColor;
                    }
                }
                break;
                
            case EnemyState.Chase:
                // Chase player
                agent.SetDestination(player.position);
                
                // If player gets too far away, return to patrol
                if (distanceToPlayer > detectionRange * 1.5f || !CanSeePlayer())
                {
                    currentState = EnemyState.Return;
                    
                    if (detectionLight != null)
                    {
                        detectionLight.color = normalColor;
                    }
                }
                
                // If within attack range, attack
                if (distanceToPlayer <= attackRange)
                {
                    // Player already takes damage on collision via PlayerController
                }
                break;
                
            case EnemyState.Return:
                // Return to patrol route
                agent.SetDestination(startPosition);
                
                if (Vector3.Distance(transform.position, startPosition) < 0.5f)
                {
                    transform.rotation = startRotation;
                    currentState = EnemyState.Patrol;
                    agent.speed = patrolSpeed;
                }
                break;
        }
    }
    
    void Patrol()
    {
        if (waiting)
            return;
            
        if (patrolPoints.Length == 0)
            return;
            
        // Check if we've reached the current patrol point
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.5f)
        {
            // Wait for a moment
            StartCoroutine(WaitAtPoint());
            
            // Move to next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }
    
    IEnumerator WaitAtPoint()
    {
        waiting = true;
        agent.isStopped = true;
        
        yield return new WaitForSeconds(waitTime);
        
        agent.isStopped = false;
        waiting = false;
    }
    
    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.position - transform.position;
        
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.transform == player)
            {
                return true;
            }
        }
        
        return false;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}