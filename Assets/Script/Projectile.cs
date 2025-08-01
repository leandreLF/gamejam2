using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 5f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ici, tu peux v�rifier si la cible est le joueur ou autre
        Debug.Log("Projectile a touch� : " + other.name);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(25f);
            Destroy(gameObject); // d�truire le projectile
        }
    }
}
