using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider))]
public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionDamage = 100f;
    public float explosionForce = 500f;
    public LayerMask explosionAffectedLayers;
    public GameObject explosionEffect;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Health health;
    private Collider barrelCollider;
    private bool hasExploded = false;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        health = GetComponent<Health>();
        barrelCollider = GetComponent<Collider>();

        if (health == null)
        {
            Debug.LogError("Health component missing on barrel!", this);
            enabled = false;
            return;
        }

        health.OnDeath += HandleBarrelDeath;
        RegisterWithGameManager();
    }

    void RegisterWithGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.barrels.Add(this);
        }
        else
        {
            Debug.LogWarning("GameManager instance not found, retrying...");
            Invoke("RegisterWithGameManager", 1f);
        }
    }

    public void ResetBarrel()
    {
        hasExploded = false;
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        gameObject.SetActive(true);

        if (health != null)
        {
            health.ResetHealth();
        }

        if (barrelCollider != null)
        {
            barrelCollider.enabled = true;
        }
    }

    private void HandleBarrelDeath()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Désactiver le collider immédiatement
        if (barrelCollider != null) barrelCollider.enabled = false;

        // Jouer l'effet d'explosion
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(effect, 3f);
        }

        // Appliquer les dégâts et la force d'explosion
        ApplyExplosionEffects();

        // Désactiver le baril après un court délai
        StartCoroutine(DisableAfterDelay());
    }

    private void ApplyExplosionEffects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionAffectedLayers);

        foreach (Collider hit in colliders)
        {
            // Appliquer des dégâts
            Health targetHealth = hit.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(explosionDamage);
            }

            // Appliquer une force physique
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

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath -= HandleBarrelDeath;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.barrels.Remove(this);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}