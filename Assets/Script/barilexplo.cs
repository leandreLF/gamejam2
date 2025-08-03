using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider))]
public class ExplosiveBarrel : ResettableObject
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionDamage = 100f;
    public float explosionForce = 500f;
    public LayerMask explosionAffectedLayers;
    public GameObject explosionEffect;

    private Health health;
    private Collider barrelCollider;
    private bool hasExploded = false;

    void Awake()
    {
        health = GetComponent<Health>();
        barrelCollider = GetComponent<Collider>();

        if (health == null)
        {
            Debug.LogError("Health component missing on barrel!", this);
            enabled = false;
            return;
        }

        health.OnDeath += HandleBarrelDeath;
    }

    void Start()
    {
        // Enregistre cet objet dans la pièce courante
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.RegisterObjectInCurrentRoom(gameObject);
        }
        else
        {
            Debug.LogWarning("RoomManager instance not found during ExplosiveBarrel Start!");
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath -= HandleBarrelDeath;
        }
    }

    public override void ResetObject()
    {
        base.ResetObject(); // Reset position/rotation/velocity

        hasExploded = false;

        if (barrelCollider != null)
            barrelCollider.enabled = true;

        if (health != null)
            health.ResetHealth();

        gameObject.SetActive(true);
    }

    private void HandleBarrelDeath()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        if (barrelCollider != null)
            barrelCollider.enabled = false;

        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(effect, 3f);
        }

        ApplyExplosionEffects();

        StartCoroutine(DisableAfterDelay());
    }

    private void ApplyExplosionEffects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionAffectedLayers);

        foreach (Collider hit in colliders)
        {
            Health targetHealth = hit.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(explosionDamage);
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f);
            }
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
