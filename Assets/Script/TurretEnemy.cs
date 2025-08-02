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
    public float shootingAnimDuration = 0.3f;

    private Transform currentTarget;
    private float nextFireTime;
    private bool isActive = false;
    private bool isShooting = false;
    private float shootingEndTime;
    private GameObject playerObject;

    void Start()
    {
        RailMover.OnGameStarted += ActivateTurret;
        Health.OnAnyEntityDied += OnEntityDied;

        if (animator == null)
            animator = GetComponent<Animator>();

        playerObject = GameObject.FindGameObjectWithTag(targetTag);
    }

    void Update()
    {
        if (!isActive) return;

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
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag(targetTag);
            if (playerObject == null) return;
        }

        if (currentTarget != null && currentTarget.gameObject.activeInHierarchy)
            return;

        if (playerObject.activeInHierarchy)
        {
            Health playerHealth = playerObject.GetComponent<Health>();
            if (playerHealth != null && !playerHealth.isDead)
            {
                currentTarget = playerObject.transform;
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
    }

    private void OnEntityDied(GameObject entity)
    {
        if (currentTarget != null && currentTarget.gameObject == playerObject)
        {
            currentTarget = null;
            isActive = false;
        }
    }
}