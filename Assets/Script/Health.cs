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

        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = true;

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.isKinematic = false;

        if (animator != null)
        {
            animator.enabled = true;
            animator.ResetTrigger(deathTrigger);
            animator.SetBool("isDead", false);
            animator.SetBool("isShooting", false);
            animator.Play("Idle", 0, 0f);
        }

        var railMover = GetComponent<RailMover>();
        if (railMover != null)
        {
            railMover.enabled = true;
            railMover.ResetPlayer(); // <-- ajoute cette ligne
        }
        foreach (var turret in GetComponentsInChildren<TurretEnemy>(true)) // (true) pour aussi récupérer les désactivés
        {
            turret.enabled = true;
        }
    }

    void Die()
    {
        if (_isDead) return;

        _isDead = true;

        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.isKinematic = true;

        if (animator != null)
        {
            animator.SetTrigger(deathTrigger);
        }

        // Désactiver RailMover si présent
        var railMover = GetComponent<RailMover>();
        if (railMover != null) railMover.enabled = false;

        //  Désactiver tous les TurretEnemy dans les enfants
        foreach (var turret in GetComponentsInChildren<TurretEnemy>())
        {
            turret.enabled = false;
        }

        OnDeath?.Invoke();
        OnAnyEntityDied?.Invoke(gameObject);

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DelayedDeactivation());
        }
        else
        {
            gameObject.SetActive(false);
        }
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
