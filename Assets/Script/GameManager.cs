using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public float respawnDelay = 1f;

    private GameObject currentPlayer;
    private int lastSpawnIndex = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SpawnPlayer();
    }

    public void PlayerDied(GameObject deadPlayer)
    {
        StartCoroutine(RespawnPlayerCoroutine());

        if (UIManager.Instance != null)
            UIManager.Instance.ShowReadyUI();
    }

    IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }

        Transform spawnPoint = GetNextSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point found.");
            return;
        }

        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        var health = currentPlayer.GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += () => PlayerDied(currentPlayer);
        }

        Debug.Log("Player spawned at: " + spawnPoint.position);
    }

    Transform GetNextSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points defined in GameManager.");
            return null;
        }

        lastSpawnIndex = (lastSpawnIndex + 1) % spawnPoints.Length;
        return spawnPoints[lastSpawnIndex];
    }
}
