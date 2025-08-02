using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Health health;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        health = GetComponent<Health>();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.enemies.Add(this);
        }
    }

    public void ResetEnemy()
    {
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        health.ResetHealth();
        gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.enemies.Remove(this);
        }
    }
}