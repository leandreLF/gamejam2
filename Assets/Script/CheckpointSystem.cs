using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateCheckpoint(Transform checkpointTransform)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerRespawn respawn = player.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.SetSpawnPoint(checkpointTransform); // Appel correct de la méthode
                Debug.Log("New checkpoint set at: " + checkpointTransform.position);
            }
        }
    }
}