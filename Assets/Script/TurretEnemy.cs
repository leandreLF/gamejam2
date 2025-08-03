using UnityEngine;
using System.Collections.Generic;

public class TurretEnemy : MonoBehaviour
{
    [Header("Targeting")]
    public string[] targetTags = { "Player", "enemy" }; // Tags multiples
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

        // Actualiser la liste des cibles à intervalle régulier
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

    public void ActivateTurret()
    {
        isActive = true;
    }

    void FindTarget()
    {
        // Si on a déjà une cible valide, on la garde
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

        // Ne pas se cibler soi-même
        if (target == gameObject || target.transform.IsChildOf(transform))
            return false;
        if (target.layer == LayerMask.NameToLayer("Smoke"))
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

        // Vérifie s'il y a une ligne de vue directe
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

        // Ajouter le script de gestion de propriétaire
        ProjectileOwner projectileOwner = projectile.AddComponent<ProjectileOwner>();
        projectileOwner.owner = gameObject;
        projectileOwner.ownerParent = transform.parent != null ? transform.parent.gameObject : null;

        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
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
        // Si l'entité morte est notre cible actuelle, on la retire
        if (currentTarget != null && currentTarget.gameObject == entity)
        {
            currentTarget = null;
        }

        // Retirer l'entité morte de la liste des cibles potentielles
        if (potentialTargets.Contains(entity))
        {
            potentialTargets.Remove(entity);
        }
    }
}

// Nouveau script à ajouter dans le même fichier
public class ProjectileOwner : MonoBehaviour
{
    public GameObject owner;        // L'objet qui a tiré (la Main)
    public GameObject ownerParent;  // Le parent de l'objet qui a tiré (Bob)

    void Start()
    {
        // Ignore les collisions avec le propriétaire et son parent
        if (owner != null)
        {
            IgnoreCollisions(owner);
        }

        if (ownerParent != null)
        {
            IgnoreCollisions(ownerParent);
        }
    }

    private void IgnoreCollisions(GameObject target)
    {
        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider == null) return;

        Collider[] targetColliders = target.GetComponentsInChildren<Collider>();
        foreach (Collider col in targetColliders)
        {
            if (col != null && col.enabled)
            {
                Physics.IgnoreCollision(projectileCollider, col);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ne pas détruire le projectile s'il touche le propriétaire ou son parent
        if (collision.gameObject == owner || collision.gameObject == ownerParent ||
            collision.transform.IsChildOf(owner.transform) ||
            (ownerParent != null && collision.transform.IsChildOf(ownerParent.transform)))
        {
            return;
        }

        // Détruire le projectile après collision avec d'autres objets
        Destroy(gameObject);
    }
}