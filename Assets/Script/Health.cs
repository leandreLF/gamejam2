using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    public bool isDead { get; private set; } = false;

    public float maxHealth => _maxHealth;
    public float currentHealth => _currentHealth;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] attachedParts;

    public event System.Action OnDeath;

    void Start()
    {
        _currentHealth = _maxHealth;
        if (animator == null) animator = GetComponent<Animator>();
        CheckInitialHealth();
    }

    private void CheckInitialHealth()
    {
        if (_currentHealth <= 0 && !isDead)
        {
            Die();
        }
        else
        {
            isDead = false;
            UpdateAnimator();
            SetAttachedPartsActive(true);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        if (_currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
        isDead = false;
        UpdateAnimator();
        SetAttachedPartsActive(true);
        CheckInitialHealth();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        UpdateAnimator();
        OnDeath?.Invoke();

        SetAttachedPartsActive(false);
        Destroy(gameObject, 0.6f);
    }

    // MODIFICATION PRINCIPALE ICI
    private void UpdateAnimator()
    {
        if (animator == null) return;

        // On utilise uniquement le booléen isDead
        animator.SetBool("isDead", isDead);
    }

    private void SetAttachedPartsActive(bool active)
    {
        if (attachedParts == null) return;

        foreach (GameObject part in attachedParts)
        {
            if (part != null)
            {
                part.SetActive(active);
            }
        }
    }
}