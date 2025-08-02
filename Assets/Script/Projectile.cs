using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject owner;  // R�f�rence au cr�ateur du projectile (la Main)
    public float damage = 20f;
    public bool destroyOnImpact = true;
    public float lifetime = 5f;

    private void Start()
    {
        // Ignore les collisions avec le propri�taire et son parent (Bob)
        if (owner != null)
        {
            // Ignore avec la Main
            Collider projectileCollider = GetComponent<Collider>();
            Collider ownerCollider = owner.GetComponent<Collider>();

            if (projectileCollider != null && ownerCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, ownerCollider);
            }

            // Ignore avec le parent de la Main (Bob)
            if (owner.transform.parent != null)
            {
                Collider[] parentColliders = owner.transform.parent.GetComponentsInChildren<Collider>();

                foreach (Collider col in parentColliders)
                {
                    if (projectileCollider != null && col != null)
                    {
                        Physics.IgnoreCollision(projectileCollider, col);
                    }
                }
            }
        }

        // Destruction automatique apr�s un d�lai
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Applique les d�g�ts � la cible
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null && collision.gameObject != owner)
        {
            targetHealth.TakeDamage(damage);
        }

        // Destruction du projectile
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}