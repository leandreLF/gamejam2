using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
        SetSpawnPoint(transform.position, transform.rotation); // Initialise avec la position de d�part
    }

    // Nouvelle m�thode avec param�tre Transform
    public void SetSpawnPoint(Transform newSpawnPoint)
    {
        if (newSpawnPoint != null)
        {
            spawnPosition = newSpawnPoint.position;
            spawnRotation = newSpawnPoint.rotation;
        }
    }

    // M�thode existante avec position + rotation
    public void SetSpawnPoint(Vector3 position, Quaternion rotation)
    {
        spawnPosition = position;
        spawnRotation = rotation;
    }

    public void Respawn()
    {
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        health.Heal(health.maxHealth);

        // Effet visuel optionnel
        Debug.Log("Player respawned at: " + spawnPosition);
    }
}