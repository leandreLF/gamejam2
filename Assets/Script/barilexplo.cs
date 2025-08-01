using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;     // Rayon d'effet de l'explosion
    public float explosionForce = 700f;    // Force de pouss�e appliqu�e aux objets rigides
    public float damage = 50f;              // D�g�ts inflig�s aux objets avec un script Health
    public LayerMask damageableLayers;     // Couches affect�es par l'explosion

    [Header("Explosion Effects")]
    public GameObject explosionEffectPrefab;  // Prefab d'effet visuel d'explosion (optionnel)

    private bool hasExploded = false;

    // Appelle cette m�thode pour d�clencher l'explosion
    public void Explode()
    {
        if (hasExploded) return;  // Ne pas exploser plusieurs fois

        hasExploded = true;

        // Instancie l'effet visuel (si assign�)
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Trouve tous les colliders dans le rayon d'explosion
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayers);

        foreach (Collider nearbyObject in colliders)
        {
            // Appliquer d�g�ts si l'objet a un script Health
            Health health = nearbyObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Appliquer force d'explosion aux objets Rigidbody
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Optionnel : d�truire le baril apr�s explosion
        Destroy(gameObject);
    }

    // Pour tester dans l'�diteur, par exemple en appuyant sur la touche E
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Explode();
        }
    }

    // Visualisation dans l'�diteur de la zone d'explosion
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
