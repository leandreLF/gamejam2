using UnityEngine;
using System.Collections;

public class RailPlayerKick : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float kickRange = 1.5f;
    public float kickCooldown = 0.5f;
    public float damage = 30f;

    public LayerMask obstacleMask;
    public Transform kickOrigin;

    private bool isKicking = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isKicking) return;

        // Détection d'obstacle devant
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, kickRange, obstacleMask))
        {
            StartCoroutine(PerformKick(hit.collider.gameObject));
        }
        else
        {
            // Avance le long du rail
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    IEnumerator PerformKick(GameObject obstacle)
    {
        isKicking = true;

        // Animation
        if (animator != null)
            animator.SetTrigger("Kick");

        // Dégâts
        Health health = obstacle.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damage);

        // Attendre un peu pour simuler le coup
        yield return new WaitForSeconds(kickCooldown);

        isKicking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (kickOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * kickRange);
        }
    }
}
