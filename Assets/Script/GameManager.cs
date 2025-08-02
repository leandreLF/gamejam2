using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public float respawnDelay = 1f;

    [Header("UI Settings")]
    public CanvasGroup restartCanvasGroup;
    public Button restartButton;

    [Header("Game Elements")]
    public List<Item> items = new List<Item>();
    public List<ExplosiveBarrel> barrels = new List<ExplosiveBarrel>();
    public List<EnemyController> enemies = new List<EnemyController>();

    private GameObject currentPlayer;
    private int lastSpawnIndex = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGameElements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(ResetLevel);
            SetRestartUI(false);
        }
    }

    void InitializeGameElements()
    {
        items.AddRange(FindObjectsOfType<Item>(true));
        barrels.AddRange(FindObjectsOfType<ExplosiveBarrel>(true));
        enemies.AddRange(FindObjectsOfType<EnemyController>(true));
    }

    public void PlayerDied(GameObject deadPlayer)
    {
        StartCoroutine(RespawnPlayerCoroutine());
        SetRestartUI(true);
    }

    IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);

        Transform spawnPoint = GetSafeSpawnPoint();
        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        if (currentPlayer.TryGetComponent<Health>(out var health))
        {
            health.OnDeath += () => PlayerDied(currentPlayer);
        }

        ClearAreaAroundSpawn(spawnPoint.position);
    }

    public void ResetLevel()
    {
        StartCoroutine(ResetLevelCoroutine());
    }

    IEnumerator ResetLevelCoroutine()
    {
        SetRestartUI(false);

        foreach (var enemy in enemies)
        {
            if (enemy != null) enemy.ResetEnemy();
        }

        foreach (var item in items)
        {
            if (item != null) item.ResetItem();
        }

        foreach (var barrel in barrels)
        {
            if (barrel != null) barrel.ResetBarrel();
        }

        yield return null;

        if (currentPlayer == null)
        {
            Transform spawnPoint = GetSafeSpawnPoint();
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

            if (currentPlayer.TryGetComponent<Health>(out var health))
            {
                health.OnDeath += () => PlayerDied(currentPlayer);
            }
        }
    }

    void SetRestartUI(bool visible)
    {
        if (restartCanvasGroup != null)
        {
            restartCanvasGroup.alpha = visible ? 1 : 0;
            restartCanvasGroup.blocksRaycasts = visible;
            restartCanvasGroup.interactable = visible;
        }
    }

    Transform GetSafeSpawnPoint()
    {
        lastSpawnIndex = (lastSpawnIndex + 1) % spawnPoints.Length;
        return spawnPoints[lastSpawnIndex];
    }

    void ClearAreaAroundSpawn(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 5f);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Enemy") && col.TryGetComponent<EnemyController>(out var enemy))
            {
                enemy.RetreatFrom(position);
            }
            else if (col.CompareTag("Projectile"))
            {
                Destroy(col.gameObject);
            }
        }
    }

    void ResetEnemies()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.ResetEnemy();
            }
        }
    }
    public void UpdateInitialStatesOfResettableObjects()
    {
        foreach (var item in items)
        {
            if (item != null)
            {
                ResettableObject resettable = item.GetComponent<ResettableObject>();
                if (resettable != null)
                    resettable.UpdateInitialStateToCurrent();
            }
        }

        foreach (var barrel in barrels)
        {
            if (barrel != null)
            {
                ResettableObject resettable = barrel.GetComponent<ResettableObject>();
                if (resettable != null)
                    resettable.UpdateInitialStateToCurrent();
            }
        }

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                ResettableObject resettable = enemy.GetComponent<ResettableObject>();
                if (resettable != null)
                    resettable.UpdateInitialStateToCurrent();
            }
        }
    }
}