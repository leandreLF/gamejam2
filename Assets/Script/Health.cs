using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    public float maxHealth => _maxHealth;
    public float currentHealth => _currentHealth;

    public event System.Action OnDeath;

    void Start() => _currentHealth = _maxHealth;

    public void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        if (_currentHealth <= 0) Die();
    }

    public void Heal(float amount) => _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);

    public void ResetHealth() => _currentHealth = _maxHealth;

    private void Die() => OnDeath?.Invoke();
}