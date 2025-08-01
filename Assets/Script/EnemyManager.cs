using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public float respawnDelay = 3f;

    private List<EnemyData> enemyData = new List<EnemyData>();

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

    public void RegisterEnemy(GameObject enemy)
    {
        enemyData.Add(new EnemyData(enemy, enemy.transform.position, enemy.transform.rotation));
    }

    public void RespawnAllEnemies()
    {
        foreach (var data in enemyData)
        {
            if (data.enemyInstance == null)
            {
                GameObject newEnemy = Instantiate(data.prefab, data.spawnPosition, data.spawnRotation);
                data.enemyInstance = newEnemy;
            }
        }
    }

    private class EnemyData
    {
        public GameObject enemyInstance;
        public GameObject prefab;
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;

        public EnemyData(GameObject enemy, Vector3 position, Quaternion rotation)
        {
            enemyInstance = enemy;
            prefab = enemy;
            spawnPosition = position;
            spawnRotation = rotation;
        }
    }
}