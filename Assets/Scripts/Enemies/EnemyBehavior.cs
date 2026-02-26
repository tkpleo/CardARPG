using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public bool isMovingXEnemy = false;
    public bool isMovingZEnemy = false;
    public float moveSpeed = 2f;
    public float moveDistance = 2f;
    public float damageTaken = 0f;
    public TextMeshProUGUI damageText;
    public GameObject AOEExplosionVFX;
    public GameObject slowAOEExplosionVFX;
    public GameObject stunAOEExplosionVFX;


    public LayerMask aoeLayerMask = ~0; // default to all layers; set to "Enemy" layer in Inspector

    private Vector3 startPosition;
    private float movementTimer = 0f;

    private bool isStunned = false;
    private bool isSlowed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        if (damageText != null)
            damageText.text = damageTaken.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingXEnemy || isMovingZEnemy)
        {
            MoveEnemy();
        }
    }

    // Public entrypoint kept for external callers (e.g. Bullet collision)
    public void TakeDamage(BulletBehavior bullet)
    {
        ApplyOnHitEffects(bullet, fromAOE: false);
    }

    // Internal helper - fromAOE prevents re-triggering the AOE when propagating damage
    private void ApplyOnHitEffects(BulletBehavior bullet, bool fromAOE)
    {
        damageTaken = bullet.bulletDamage;
        if (bullet.isStunBullet)
        {
            isStunned = true;
            StartCoroutine(StunEffect());
        }
        if (bullet.isSlowBullet)
        {
            isSlowed = true;
            StartCoroutine(SlowEffect());
        }

        if (bullet.isOpportunistBullet && (isStunned == true || isSlowed == true))
        {
            damageTaken *= 2;
            Debug.Log("Enemy took opportunist damage");
        }

        Debug.Log("Enemy took damage: " + damageTaken);
        if (damageText != null)
            damageText.text = damageTaken.ToString();

        // Only trigger AOE once (when this call is the original hit)
        if (bullet.isAOEBullet && !fromAOE)
        {
            if (bullet.isSlowBullet)
            {
                Instantiate(slowAOEExplosionVFX, transform.position, Quaternion.identity);
                if (bullet.isStunBullet)
                {
                    Instantiate(stunAOEExplosionVFX, transform.position, Quaternion.identity);
                }
            }
            if (bullet.isStunBullet)
            {
                Instantiate(stunAOEExplosionVFX, transform.position, Quaternion.identity);
            }
            else
            { 
                Instantiate(AOEExplosionVFX, transform.position, Quaternion.identity);
            }
            // Use the configured layer mask and radius. This avoids passing a layer index like '6'.
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, bullet.AOERadius, aoeLayerMask);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider == null)
                    continue;

                // Try to find an EnemyBehavior on the collider or its parent
                EnemyBehavior enemy = hitCollider.GetComponent<EnemyBehavior>() ?? hitCollider.GetComponentInParent<EnemyBehavior>();
                if (enemy == null)
                    continue;

                // Avoid re-applying to self
                if (enemy == this)
                    continue;

                // Propagate damage but mark as fromAOE to avoid cascading AOE
                enemy.ApplyOnHitEffects(bullet, fromAOE: true);
            }
        }
    }

    private IEnumerator SlowEffect()
    {
        yield return new WaitForSeconds(4f);
        isSlowed = false;
    }
    private IEnumerator StunEffect()
    {
        yield return new WaitForSeconds(2f);
        isStunned = false;
    }

    private void MoveEnemy()
    {
        // If stunned, don't modify position or the movement timer — this freezes the enemy in place
        if (isStunned)
            return;
        if (isSlowed)
        {
            // Slow movement speed by half when slowed
            movementTimer += Time.deltaTime * (moveSpeed / 2f);
        }
        else
        {
            // Advance a local movement timer instead of using Time.time directly.
            // This preserves the current phase when paused (stunned) so the enemy doesn't snap back.
            movementTimer += Time.deltaTime * moveSpeed;
        }
        if (isMovingXEnemy)
        {
            float xPosition = startPosition.x + Mathf.PingPong(movementTimer, moveDistance * 2) - moveDistance;
            transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
        }
        if (isMovingZEnemy)
        {
            float zPosition = startPosition.z + Mathf.PingPong(movementTimer, moveDistance * 2) - moveDistance;
            transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        }

    }
}

