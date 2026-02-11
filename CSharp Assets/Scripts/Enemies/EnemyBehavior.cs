using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public bool isMovingEnemy = false;
    public float moveSpeed = 2f;
    public float moveDistance = 2f;
    public int lastDamageTaken = 0;
    public TextMeshProUGUI damageText;

    // AOE settings - configurable in the Inspector
    public float aoeRadius = 30f;
    public LayerMask aoeLayerMask = ~6; // default to all layers; set to your "Enemy" layer in Inspector

    private Vector3 startPosition;
    private float movementTimer = 0f;

    private bool isStunned = false;
    private bool isSlowed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        if (damageText != null)
            damageText.text = lastDamageTaken.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingEnemy)
        {   
            MoveEnemy();
        }
    }

    // Public entrypoint kept for external callers (e.g. Bullet collision)
    public void TakeDamage(Bullet bullet)
    {
        ApplyDamage(bullet, fromAOE: false);
    }

    // Internal helper - fromAOE prevents re-triggering the AOE when propagating damage
    private void ApplyDamage(Bullet bullet, bool fromAOE)
    {
        Debug.Log("Enemy took damage: " + bullet.bulletDamage);
        lastDamageTaken = bullet.bulletDamage;
        if (damageText != null)
            damageText.text = lastDamageTaken.ToString();

        if (bullet.isStunBullet)
        {
            StartCoroutine(StunEffect());
        }
        if (bullet.isSlowBullet)
        {
            StartCoroutine(SlowEffect());
        }

        // Only trigger AOE once (when this call is the original hit)
        if (bullet.isAOEBullet && !fromAOE)
        {
            // Use the configured layer mask and radius. This avoids passing a layer index like '6'.
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoeRadius, aoeLayerMask);
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
                enemy.ApplyDamage(bullet, fromAOE: true);
            }
        }
    }

    private IEnumerator SlowEffect() 
    {
        isSlowed = true;
        yield return new WaitForSeconds(4f);
        isSlowed = false;
    }
    private IEnumerator StunEffect() 
    {
        isStunned = true;
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
        
        float xPosition = startPosition.x + Mathf.PingPong(movementTimer, moveDistance * 2) - moveDistance;
        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
    }
}