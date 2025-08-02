using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerRespawn : MonoBehaviour
{
    private Health health;
    private RailMover railMover;
    private Transform currentSpawnPoint;

    void Start()
    {
        health = GetComponent<Health>();
        railMover = GetComponent<RailMover>();
        currentSpawnPoint = transform;
        health.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        if (health != null) health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (GameOverUI.Instance != null) GameOverUI.Instance.ShowGameOver();
    }

    public void SetSpawnPoint(Transform newSpawnPoint)
    {
        currentSpawnPoint = newSpawnPoint;
    }

    public void ResetPlayer()
    {
        if (currentSpawnPoint != null)
        {
            transform.position = currentSpawnPoint.position;
            transform.rotation = currentSpawnPoint.rotation;
        }

        if (health != null) health.ResetHealth();
        if (railMover != null) railMover.ResetPlayer();
    }
}