using System;
using UnityEngine;

public class BulletBehavior : MonoBehaviour, IPoolable<BulletBehavior>
{
    [SerializeField]
    private BulletData bulletData;

    public float bulletSpeed => bulletData != null ? bulletData.DefaultSpeed : 10f;
    public float bulletDamage => bulletData != null ? bulletData.DefaultDamage : 1f;

    public bool isStunBullet = false;
    public bool isAOEBullet = false;
    public float AOERadius = 0f;
    public bool isSlowBullet = false;
    public bool isOpportunistBullet = false;

    public event Action<BulletBehavior> ReturnRequested;

    private void FixedUpdate()
    {
        transform.Translate(bulletSpeed * Time.fixedDeltaTime * Vector3.forward);
    }

    public void Initialize(BulletData data)
    {
        bulletData = data;
    }

    #region Collision Handling
    private void OnCollisionEnter(Collision collision) => HandleCollision(collision.gameObject);
    private void HandleCollision(GameObject collidedObject)
    {
        if (collidedObject.CompareTag("Enemy"))
            collidedObject.GetComponent<EnemyBehavior>().TakeDamage(this);

        ReturnRequested?.Invoke(this);
    }

    public void OnRetrieved()
    {
        // Reset any transient state here if needed (e.g. visual effects, timers).
    }
    #endregion
}
