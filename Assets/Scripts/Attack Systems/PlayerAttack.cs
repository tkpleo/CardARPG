using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletSpawnPoint;
    [SerializeField, LabelOverride("Bullet Type"), Tooltip("The Scriptable Object of the bullet intedended to be fired.")]
    private BulletData bulletPrefab;

    BulletAbilityManager bulletAbilityManager;

    private void OnEnable()
    {
        if (bulletPrefab != null)
            BulletPooler.Prewarm(bulletPrefab, 10);
        else
            Debug.LogWarning("Bullet prefab not assigned in PlayerAttack.");
    }

    public void InitAttack()
    {
        bulletAbilityManager = GetComponent<BulletAbilityManager>();
        if (bulletPrefab == null || bulletSpawnPoint == null) return;

        // Get a pooled bullet (inactive)
        BulletBehavior pooledBullet = BulletPooler.GetObject(bulletPrefab);
        if (pooledBullet == null) return;

        // Position and rotate before activation so transform is correct when enabled
        pooledBullet.transform.position = bulletSpawnPoint.transform.position;
        pooledBullet.transform.rotation = bulletSpawnPoint.transform.rotation;

        // Activate (this triggers OnEnable in BulletBehavior to apply default BulletData)
        pooledBullet.gameObject.SetActive(true);

        // Apply player abilities/overrides after activation so they take precedence
        if (bulletAbilityManager != null)
        {
            bulletAbilityManager.BulletBuilder(pooledBullet);
        }
    }
}
