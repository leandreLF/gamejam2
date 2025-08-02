using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Health health;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();

        // Sauvegarder la position et rotation initiales
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void ResetEnemy()
    {
        // R�initialiser l'agent de navigation
        if (navAgent != null)
        {
            navAgent.Warp(initialPosition);
            navAgent.ResetPath();
        }
        else
        {
            transform.position = initialPosition;
        }

        // R�initialiser la rotation
        transform.rotation = initialRotation;

        // R�initialiser la sant� si disponible
        if (health != null)
        {
            health.ResetHealth();
        }

        // R�activer l'objet
        gameObject.SetActive(true);
    }

    public void RetreatFrom(Vector3 position)
    {
        Vector3 retreatDirection = (transform.position - position).normalized;
        Vector3 retreatPosition = transform.position + retreatDirection * 3f;

        // Utiliser le NavMeshAgent si disponible
        if (navAgent != null && navAgent.isActiveAndEnabled)
        {
            navAgent.SetDestination(retreatPosition);
        }
        else
        {
            transform.position = retreatPosition;
        }
    }
}