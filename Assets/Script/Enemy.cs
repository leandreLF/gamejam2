using UnityEngine;
using System.Collections;

public class TurretEnemy : MonoBehaviour
{
    [Header("Cible")]
    public string targetTag = "Player";
    public float detectionRadius = 15f;
    private Transform target;

    [Header("Tir")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float projectileSpeed = 10f;

    private Animator animator;
    private float fireTimer;
    private bool isInSmoke = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        FindTarget();

        if (target == null || isInSmoke)
        {
            SetShooting(false);
            return;
        }

        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null && targetHealth.isDead)
        {
            target = null;
            SetShooting(false);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > detectionRadius)
        {
            SetShooting(false);
            return;
        }

        // **Ne pas tourner le transform !**
        // On calcule juste la direction vers la cible :
        Vector3 dir = target.position - firePoint.position;
        dir.y = 0;
        dir.Normalize();

        // Flip sprite horizontal (optionnel) si tu veux que le sprite "regarde" à gauche ou droite :
        if (target.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        // Tir automatique
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Fire(dir);
            fireTimer = 0f;
        }
    }

    void Fire(Vector3 direction)
    {
        SetShooting(true);

        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileSpeed;
            }
        }

        StartCoroutine(ResetShootFlag());
    }

    private IEnumerator ResetShootFlag()
    {
        yield return new WaitForSeconds(0.1f);
        SetShooting(false);
    }

    private void SetShooting(bool state)
    {
        if (animator != null)
        {
            animator.SetBool("isShooting", state);
        }
    }

    public void SetInSmoke(bool value)
    {
        isInSmoke = value;
    }

    void FindTarget()
    {
        if (target != null) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                target = hit.transform;
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
