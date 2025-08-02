using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float deathDelay = 1f;
    [SerializeField] private string deathTrigger = "Die";
    private float _currentHealth;
    private bool _isDead;
    private Animator animator;

    public float maxHealth => _maxHealth;
    public float currentHealth => _currentHealth;
    public bool isDead => _isDead;

    public event Action OnDeath;
    public static event Action<GameObject> OnAnyEntityDied;

    void Start()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHealth = Mathf.Max(_currentHealth - damage, 0);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
        gameObject.SetActive(true);

        // Réactiver les composants
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = true;

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.isKinematic = false;

        // Réactiver l'Animator
        if (animator != null)
        {
            animator.enabled = true;
            animator.ResetTrigger(deathTrigger);
        }

        // Réactiver les scripts de contrôle
        var railMover = GetComponent<RailMover>();
        if (railMover != null) railMover.enabled = true;
    }

    void Die()
    {
        _isDead = true;

        // Désactiver les composants physiques
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.isKinematic = true;

        // Déclencher l'animation de mort
        if (animator != null)
        {
            animator.SetTrigger(deathTrigger);
        }

        // Désactiver les scripts de contrôle
        var railMover = GetComponent<RailMover>();
        if (railMover != null) railMover.enabled = false;

        // Lancer les événements de mort
        OnDeath?.Invoke();
        OnAnyEntityDied?.Invoke(gameObject);

        // Démarrer la coroutine de désactivation
        StartCoroutine(DelayedDeactivation());
    }

    private IEnumerator DelayedDeactivation()
    {
        yield return new WaitForSeconds(deathDelay);

        if (animator != null) animator.enabled = false;

        gameObject.SetActive(false);
    }
    public void Revive()
    {
        _isDead = false;
    }
}