using UnityEngine;

public class BulletPooler : ObjectFactory<BulletPooler, BulletBehavior>
{
    /// <summary>
    /// Pre-warm a pool for the bullet type described by <paramref name="data"/>.
    /// The ScriptableObject should contain a prefab with a `BulletBehavior` component.
    /// </summary>
    public static void Prewarm(BulletData data, int count)
    {
        if (data == null)
        {
            Debug.LogWarning("BulletPooler.Prewarm called with null BulletData.");
            return;
        }

        if (data.BulletPrefab == null)
        {
            Debug.LogWarning($"BulletData '{data.name}' has no BulletPrefab assigned. Cannot prewarm pool.");
            return;
        }

        if (!data.BulletPrefab.TryGetComponent<BulletBehavior>(out var prefabBehavior))
        {
            Debug.LogWarning($"Prefab '{data.BulletPrefab.name}' does not contain a BulletBehavior component. Cannot prewarm pool.");
            return;
        }

        Prewarm(prefabBehavior, count);
    }

    /// <summary>
    /// Get a pooled bullet instance for the provided BulletData.
    /// The returned bullet will have its `Initialize(BulletData)` called so the instance uses the runtime data.
    /// It will NOT be activated — callers should position/rotate then set active (matches existing PlayerAttack flow).
    /// </summary>
    public static BulletBehavior GetObject(BulletData data)
    {
        if (data == null)
        {
            Debug.LogWarning("BulletPooler.GetObject called with null BulletData.");
            return null;
        }

        if (data.BulletPrefab == null)
        {
            Debug.LogWarning($"BulletData '{data.name}' has no BulletPrefab assigned. Cannot get pooled bullet.");
            return null;
        }

        if (!data.BulletPrefab.TryGetComponent<BulletBehavior>(out var prefabBehavior))
        {
            Debug.LogWarning($"Prefab '{data.BulletPrefab.name}' does not contain a BulletBehavior component. Cannot get pooled bullet.");
            return null;
        }

        BulletBehavior instance = GetObject(prefabBehavior);
        if (instance == null)
        {
            Debug.LogError("BulletPooler failed to retrieve an instance from the pool.");
            return null;
        }

        // Apply data now so the instance behaves correctly when enabled
        instance.Initialize(data);
        return instance;
    }
}
