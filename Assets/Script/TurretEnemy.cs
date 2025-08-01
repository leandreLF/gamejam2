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

    private Transform currentTarget;
    private float nextFireTime;
    private bool isActive = false; // Désactivé par défaut

    void Start()
    {
        RailMover.OnGameStarted += ActivateTurret; // Abonnement à l'événement
    }

    void Update()
    {
        if (!isActive) return; // Ne rien faire si inactif

        FindTarget();

        if (currentTarget == null) return;

        if (Time.time >= nextFireTime && HasLineOfSight(currentTarget))
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void ActivateTurret()
    {
        isActive = true; // Activé seulement quand le jeu commence
        Debug.Log("Turret activated!");
    }

    void FindTarget()
    {
        currentTarget = GameObject.FindGameObjectWithTag(targetTag)?.transform;
    }

    bool HasLineOfSight(Transform target)
    {
        if (target == null) return false;

        Vector3 direction = target.position - firePoint.position;
        float distance = direction.magnitude;
        direction.Normalize();

        // Debug visuel
        Debug.DrawRay(firePoint.position, direction * distance, Color.yellow, 0.1f);

        return !Physics.Raycast(firePoint.position, direction, distance, obstacleLayers);
    }

    void Fire()
    {
        if (currentTarget == null) return;

        if (muzzleFlash != null)
            muzzleFlash.Play();

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position,
            Quaternion.LookRotation(currentTarget.position - firePoint.position));

        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = (currentTarget.position - firePoint.position).normalized * projectileSpeed;
        }

        Destroy(projectile, 5f);
    }

    void OnDestroy()
    {
        RailMover.OnGameStarted -= ActivateTurret; // Désabonnement important
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}