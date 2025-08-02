using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    [Header("Targeting")]
    public string targetTag = "Player";
    public float detectionRange = 15f;
    public LayerMask obstacleLayers;
    public float aimingAngle = 45f;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float projectileSpeed = 15f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;

    [Header("Animation")]
    public Animator animator;
    public float shootingAnimDuration = 0.3f; // Durée de l'animation de tir

    private Transform currentTarget;
    private float nextFireTime;
    private bool isActive = false;
    private bool isShooting = false;
    private float shootingEndTime;

    void Start()
    {
        RailMover.OnGameStarted += ActivateTurret;
        Health.OnPlayerDied += OnPlayerDied;

        // Initialisation de l'animator si non assigné
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isActive) return;

        // Gestion de l'état de tir
        UpdateShootingState();

        FindTarget();

        if (currentTarget == null) return;

        if (Time.time >= nextFireTime && HasLineOfSight(currentTarget))
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void UpdateShootingState()
    {
        // Désactive l'état de tir après la durée de l'animation
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
        Debug.Log("Turret activated!");
    }

    void FindTarget()
    {
        if (currentTarget != null && currentTarget.gameObject.activeInHierarchy)
            return;

        GameObject player = GameObject.FindGameObjectWithTag(targetTag);
        if (player != null && player.activeInHierarchy)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null && !playerHealth.isDead)
            {
                currentTarget = player.transform;
            }
            else
            {
                currentTarget = null;
            }
        }
        else
        {
            currentTarget = null;
        }
    }

    bool HasLineOfSight(Transform target)
    {
        if (target == null) return false;

        Vector3 direction = target.position - firePoint.position;
        float distance = direction.magnitude;
        direction.Normalize();

        Debug.DrawRay(firePoint.position, direction * distance, Color.yellow, 0.1f);

        return !Physics.Raycast(firePoint.position, direction, distance, obstacleLayers);
    }

    void Fire()
    {
        if (currentTarget == null) return;

        // Déclenche l'animation de tir
        isShooting = true;
        shootingEndTime = Time.time + shootingAnimDuration;
        UpdateAnimator();

        // Effets de tir
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Création du projectile
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(currentTarget.position - firePoint.position)
        );

        // Physique du projectile
        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = (currentTarget.position - firePoint.position).normalized * projectileSpeed;
        }

        // Nettoyage
        Destroy(projectile, 5f);
    }

    void OnDestroy()
    {
        RailMover.OnGameStarted -= ActivateTurret;
        Health.OnPlayerDied -= OnPlayerDied;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    private void OnPlayerDied(GameObject playerObject)
    {
        // Vérifie si la cible morte est notre cible actuelle
        if (currentTarget != null && currentTarget.gameObject == playerObject)
        {
            Debug.Log("Player died, clearing target");
            currentTarget = null;
            isActive = false; // Désactive la tourelle
        }
    }
}