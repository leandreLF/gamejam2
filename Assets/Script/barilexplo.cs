using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
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
            GameManager.Instance.barrels.Add(this);
        }
    }

    public void ResetBarrel()
    {
        transform.SetPositionAndRotation(initialPosition, initialRotation);

        if (health != null)
        {
            health.ResetHealth();
        }

        gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.barrels.Remove(this);
        }
    }
}