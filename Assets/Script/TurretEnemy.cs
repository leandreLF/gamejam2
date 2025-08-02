using UnityEngine;
using System.Collections.Generic;

public class TurretEnemy : MonoBehaviour
{
    [Header("Targeting")]
    public string[] targetTags = { "Player", "Enemy" }; // Tags multiples
    public float detectionRange = 15f;
    public LayerMask obstacleLayers;
    public float aimingAngle = 45f;
    [SerializeField] private float targetRefreshRate = 0.5f; // Actualisation des cibles

    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float projectileSpeed = 15f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;

    [Header("Animation")]
    public Animator animator;
    public float shootingAnimDuration = 0.3f;

    private Transform currentTarget;
    private float nextFireTime;
    private bool isActive = false;
    private bool isShooting = false;
    private float shootingEndTime;
    private float lastTargetRefreshTime;
    private List<GameObject> potentialTargets = new List<GameObject>();

    void Start()
    {
        RailMover.OnGameStarted += ActivateTurret;
        Health.OnAnyEntityDied += OnEntityDied;

        if (animator == null)
            animator = GetComponent<Animator>();

        RefreshTargetList();
    }

    void Update()
    {
        if (!isActive) return;

        UpdateShootingState();

        // Actualiser la liste des cibles � intervalle r�gulier
        if (Time.time - lastTargetRefreshTime > targetRefreshRate)
        {
            RefreshTargetList();
            lastTargetRefreshTime = Time.time;
        }

        FindTarget();

        if (currentTarget == null) return;

        if (Time.time >= nextFireTime && HasLineOfSight(currentTarget))
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void RefreshTargetList()
    {
        potentialTargets.Clear();

        foreach (string tag in targetTags)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            potentialTargets.AddRange(taggedObjects);
        }
    }

    void UpdateShootingState()
    {
        if (isShooting && Time.time >= shootingEndTime)
        {
            isShooting = false;
            UpdateAnimator();
        }
    }

    void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("isShooting", isShooting);
        }
    }

    void ActivateTurret()
    {
        isActive = true;
    }

    void FindTarget()
    {
        // Si on a d�j� une cible valide, on la garde
        if (currentTarget != null && IsValidTarget(currentTarget.gameObject))
            return;

        currentTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject target in potentialTargets)
        {
            if (target == null) continue;

            if (IsValidTarget(target))
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < closestDistance && distance <= detectionRange)
                {
                    closestDistance = distance;
                    currentTarget = target.transform;
                }
            }
        }
    }

    bool IsValidTarget(GameObject target)
    {
        if (target == null || !target.activeInHierarchy)
            return false;

        Health targetHealth = target.GetComponent<Health>();
        return targetHealth != null && !targetHealth.isDead;
    }

    bool HasLineOfSight(Transform target)
    {
        if (target == null) return false;

        Vector3 direction = target.position - firePoint.position;
        float distance = direction.magnitude;
        direction.Normalize();

        Debug.DrawRay(firePoint.position, direction * distance, Color.yellow, 0.1f);

        // V�rifie s'il y a une ligne de vue directe
        return !Physics.Raycast(firePoint.position, direction, distance, obstacleLayers);
    }

    void Fire()
    {
        if (currentTarget == null) return;

        isShooting = true;
        shootingEndTime = Time.time + shootingAnimDuration;
        UpdateAnimator();

        if (muzzleFlash != null)
            muzzleFlash.Play();

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(currentTarget.position - firePoint.position)
        );

        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
            // CORRECTION: Utiliser rb.velocity au lieu de rb.linearVelocity
            rb.linearVelocity = (currentTarget.position - firePoint.position).normalized * projectileSpeed;
        }

        Destroy(projectile, 5f);
    }

    void OnDestroy()
    {
        RailMover.OnGameStarted -= ActivateTurret;
        Health.OnAnyEntityDied -= OnEntityDied;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }

    private void OnEntityDied(GameObject entity)
    {
        // Si l'entit� morte est notre cible actuelle, on la retire
        if (currentTarget != null && currentTarget.gameObject == entity)
        {
            currentTarget = null;
        }

        // Retirer l'entit� morte de la liste des cibles potentielles
        if (potentialTargets.Contains(entity))
        {
            potentialTargets.Remove(entity);
        }
    }
}