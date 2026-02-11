using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject bulletSpawnPoint;
    public GameObject bulletPrefab;
    public GameObject Player;
    BulletAbilityManager bulletAbilityManager;

    public void InitAttack()
    {
        bulletAbilityManager = GetComponent<BulletAbilityManager>();
        GameObject spawned = ObjectPoolManager.SpawnObject(bulletPrefab, bulletSpawnPoint.transform.position, Player.transform.rotation, 10, ObjectPoolManager.PoolType.Bullets);
        Bullet spawnedBullet = spawned.GetComponent<Bullet>();
        if (spawnedBullet != null)
        {
            bulletAbilityManager.BulletBuilder(spawnedBullet);
        }

    }
}
