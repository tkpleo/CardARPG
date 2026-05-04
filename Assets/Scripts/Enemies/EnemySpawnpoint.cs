using System;
using UnityEngine;

public class EnemySpawnpoint : MonoBehaviour
{
    [SerializeField] private EnemyBehavior[] enemies;
    public EnemyBehavior spawnedEnemy;
    public static Action HandleDeathCleanup;

    private void OnEnable()
    {
        SpawnEnemyRandomly();
        if (spawnedEnemy != null)
        {
            spawnedEnemy.HandleDeathCleanup += OnEnemyDeath;
        }
    }

    private void OnDisable()
    {
        if (spawnedEnemy != null)
        {
            spawnedEnemy.HandleDeathCleanup -= OnEnemyDeath;
            Destroy(spawnedEnemy.gameObject);
             spawnedEnemy = null;
        }
    }

    private void OnEnemyDeath()
    {
        HandleDeathCleanup?.Invoke();
        HandleDeathCleanup = null; // Clear the event to prevent memory leaks
        spawnedEnemy = null; // Clear reference to the dead enemy
    }

    private void SpawnEnemyRandomly()
    {
        if (enemies.Length == 0)
        {
            Debug.LogWarning("No enemies assigned to the spawn point.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, enemies.Length);
        EnemyBehavior enemyPrefab = enemies[randomIndex];
        spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}
