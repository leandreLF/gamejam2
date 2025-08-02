using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
        SetSpawnPoint(transform.position, transform.rotation);
    }

    public void SetSpawnPoint(Transform newSpawnPoint)
    {
        if (newSpawnPoint != null)
        {
            spawnPosition = newSpawnPoint.position;
            spawnRotation = newSpawnPoint.rotation;
        }
    }

    public void SetSpawnPoint(Vector3 position, Quaternion rotation)
    {
        spawnPosition = position;
        spawnRotation = rotation;
    }

    public void Respawn()
    {
        // Réinitialiser la santé
        if (health != null)
        {
            // Utilisez ResetHealth() au lieu de Heal()
            health.ResetHealth();
        }

        transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        // Réinitialiser le RailMover
        RailMover railMover = GetComponent<RailMover>();
        if (railMover != null)
        {
            railMover.ResetPlayer();
        }
    }
}