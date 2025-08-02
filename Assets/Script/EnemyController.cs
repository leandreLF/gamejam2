using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float patrolRange = 5f;
    public float moveSpeed = 3f;
    public float detectionRadius = 10f;

    private NavMeshAgent navAgent;
    private Health health;
    private Animator animator;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isDead = false;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (navAgent != null)
        {
            navAgent.speed = moveSpeed;
        }

        health.OnDeath += HandleDeath;
    }

    void Update()
    {
        if (isDead) return;

        PatrolBehavior();
    }

    private void PatrolBehavior()
    {
        if (navAgent == null || navAgent.remainingDistance < 0.5f)
        {
            Vector3 randomPoint = initialPosition + Random.insideUnitSphere * patrolRange;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRange, NavMesh.AllAreas))
            {
                if (navAgent != null) navAgent.SetDestination(hit.position);
            }
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        if (navAgent != null) navAgent.isStopped = true;
        if (animator != null) animator.SetTrigger("Die");
    }

    public void ResetEnemy()
    {
        isDead = false;

        // Réinitialisation physique
        if (navAgent != null)
        {
            navAgent.Warp(initialPosition);
            navAgent.isStopped = false;
        }
        else
        {
            transform.position = initialPosition;
        }

        transform.rotation = initialRotation;

        // Réinitialisation santé
        if (health != null)
        {
            health.ResetHealth();
        }

        // Réinitialisation animation
        ResetEnemyState();
    }

    // NOUVELLE MÉTHODE
    public void ResetEnemyState()
    {
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.SetBool("isDead", false);
            animator.SetTrigger("Idle");
        }
    }
    public void RetreatFrom(Vector3 position)
    {
        if (navAgent != null)
        {
            Vector3 direction = (transform.position - position).normalized;
            Vector3 retreatPosition = transform.position + direction * 5f; // Ajuste la distance si nécessaire

            NavMeshHit hit;
            if (NavMesh.SamplePosition(retreatPosition, out hit, 5f, NavMesh.AllAreas))
            {
                navAgent.SetDestination(hit.position);
            }
        }
    }
}