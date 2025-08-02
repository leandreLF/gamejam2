using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    public RailMover player;
    public CanvasGroup restartCanvasGroup;
    public float freezeDelay = 0.5f; // Délai avant freeze pour laisser l'UI s'afficher

    [Header("Level Reset")]
    public List<EnemyController> enemies = new List<EnemyController>();
    public List<Item> items = new List<Item>();
    public List<ExplosiveBarrel> barrels = new List<ExplosiveBarrel>();

    private Vector3 playerInitialPosition;
    private Quaternion playerInitialRotation;
    private Button restartButton;
    private bool isRestarting = false;

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

        if (player != null)
        {
            playerInitialPosition = player.transform.position;
            playerInitialRotation = player.transform.rotation;
        }

        if (restartCanvasGroup != null)
        {
            restartButton = restartCanvasGroup.GetComponentInChildren<Button>();
            SetRestartUI(false);
        }
    }

    void Start()
    {
        SetRestartUI(false);

        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath += OnPlayerDeath;
            }
        }
    }

    void OnPlayerDeath()
    {
        if (isRestarting) return;
        isRestarting = true;

        StartCoroutine(HandlePlayerDeath());
    }

    private IEnumerator HandlePlayerDeath()
    {
        Debug.Log("Player death detected");

        // 1. Afficher l'UI immédiatement
        SetRestartUI(true);

        // 2. Attendre un court délai pour s'assurer que l'UI est rendue
        yield return new WaitForSecondsRealtime(1f);

        // 3. Freeze le jeu après le délai
        Time.timeScale = 0f;
        Debug.Log("Game frozen");
    }

    private void SetRestartUI(bool visible)
    {
        if (restartCanvasGroup == null)
        {
            Debug.LogError("RestartCanvasGroup is missing!");
            return;
        }

        restartCanvasGroup.alpha = visible ? 1f : 0f;
        restartCanvasGroup.blocksRaycasts = visible;
        restartCanvasGroup.interactable = visible;

        Debug.Log($"Restart UI set to: {visible}");
    }

    public void RestartLevel()
    {
        if (!isRestarting) return;

        Debug.Log("Restarting level...");
        isRestarting = false;
        Time.timeScale = 1f;

        StartCoroutine(ResetLevelCoroutine());
    }

    private IEnumerator ResetLevelCoroutine()
    {
        // Désactiver l'UI immédiatement
        SetRestartUI(false);

        // Attendre une frame pour s'assurer que Time.timeScale est rétabli
        yield return null;

        ResetPlayer();
        ResetEnemies();
        ResetItems();
        ResetBarrels();
    }

    void ResetPlayer()
    {
        if (player == null) return;

        player.transform.SetPositionAndRotation(playerInitialPosition, playerInitialRotation);

        // Utilisez reflection pour appeler ResetPlayer si nécessaire
        var method = player.GetType().GetMethod("ResetPlayer");
        if (method != null)
        {
            method.Invoke(player, null);
        }
        else
        {
            Debug.LogError("ResetPlayer method not found on RailMover!");
        }
    }

    void ResetEnemies()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.ResetEnemy();
                Debug.Log($"Enemy {enemy.name} reset");
            }
        }
    }

    void ResetItems()
    {
        foreach (Item item in items)
        {
            if (item != null)
            {
                item.ResetItem();
                Debug.Log($"Item {item.name} reset");
            }
        }
    }

    void ResetBarrels()
    {
        foreach (ExplosiveBarrel barrel in barrels)
        {
            if (barrel != null)
            {
                barrel.ResetBarrel();
                Debug.Log($"Barrel {barrel.name} reset");
            }
        }
    }
}