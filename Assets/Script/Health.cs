using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Param�tres de vie")]
    public float maxHealth = 100f;
    private float currentHealth;

    public bool isDead => currentHealth <= 0;

    public Animator animator;

    private bool isDying = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead || isDying)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} a pris {amount} d�g�ts, sant� restante : {currentHealth}");

        if (isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;

        Debug.Log($"{gameObject.name} est mort.");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // D�sactive tous les scripts sauf celui-ci (pour finir la mort)
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }

        // D�sactive le collider pour ne plus �tre d�tect� ou bloquer
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Change le tag et/ou layer si besoin pour ignorer la d�tection
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("IgnoreRaycast");

        // D�truire l�objet apr�s 2 secondes (le temps de l�animation de mort)
        Destroy(gameObject, 0.2f);
    }
}
