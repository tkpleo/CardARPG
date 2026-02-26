using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Bullets/New Bullet Type", order = 1)]
public class BulletData : ScriptableObject
{
    #region Inspector Elements
    [Header("Default Bullet Stats")]
    [SerializeField, Range(1, 10), LabelOverride("Damage")]
    private float defaultDamage = 1f;
    [SerializeField, Range(1, 20), LabelOverride("Bullet Speed")]
    private float defaultSpeed = 10f;

    [Header("Prefab & Visuals")]
    [Tooltip("Prefab that contains mesh renderer, collider, BulletBehavior, etc.")]
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private GameObject impactVFX;
    [SerializeField]
    private AudioClip impactSfx;
    [SerializeField, Range(0.1f, 60f)]
    private float lifeTime = 5f;
    #endregion

    // Read-only accessors for runtime use
    public float DefaultDamage => defaultDamage;
    public float DefaultSpeed => defaultSpeed;

    // Visual / prefab accessors
    public GameObject BulletPrefab => bulletPrefab;
    public GameObject ImpactVFX => impactVFX;
    public AudioClip ImpactSfx => impactSfx;
    public float LifeTime => lifeTime;

    /// <summary>
    /// Helper that instantiates the configured prefab (or returns a fallback GameObject when prefab is missing).
    /// The pool/factory should call this and then assign this BulletData to the spawned BulletBehavior.
    /// </summary>
    public GameObject CreateInstance(Vector3 position, Quaternion rotation)
    {
        GameObject instance;
        if (bulletPrefab != null)
        {
            instance = Object.Instantiate(bulletPrefab, position, rotation);
        }
        else throw new System.Exception($"BulletData '{name}' is missing a bullet prefab reference. Please assign one in the inspector.");

        return instance;
    }

    private void OnValidate()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning($"BulletData '{name}' has no BulletPrefab assigned. This will cause runtime errors when the pooler tries to use this data.");
        }

        if (bulletPrefab != null && !bulletPrefab.TryGetComponent<BulletBehavior>(out _))
        {
            Debug.LogWarning($"BulletData '{name}' has a BulletPrefab assigned that does not contain a BulletBehavior component. This may cause runtime errors when the pooler tries to use this prefab.");
        }
    }
}
