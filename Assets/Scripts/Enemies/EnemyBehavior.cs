using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{

    [HideInInspector] 
    public NavMeshAgent agent;

    
    [Header("Health")]
    public float maxHealth = 20f;
    public float currentHealth = 20f;
    public float currentHP => currentHealth;
    public float maxHP => maxHealth;

    public bool IsAlive => currentHealth > 0f && gameObject != null && gameObject.activeInHierarchy;

    [Header("Targeting")]
    protected float detectionRange = 10f;
    public bool showDetectionGizmo = true;
    protected SphereCollider detectionCollider;

    [Header("Attack")]
    public float damage = 5f;
    public BoxCollider attackCollider;
    public Vector3 attackBoxSize = new Vector3(2f, 1f, 2f);
    public float attackBoxDistance = 1.5f;
    public float attackInterval = 1f;
    public float attackDuration = 0.5f;
    public bool showAttackGizmo = true;

    [HideInInspector]
    public bool isAttackBoxActive = false;
    [HideInInspector]
    public bool hasFiredLowHealth = false;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool isMoving = false;

    private Transform playerTarget;
    public Transform PlayerTarget
    {
        get => playerTarget;
        set => playerTarget = value;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

    }

    public virtual void Attack()
    {
        EnableAttackHitbox();
    }

    public virtual void AttackEnd()
    {
        DisableAttackHitbox();
    }

    public void LoseHP(float damage)
    {
        float previousHealth = currentHealth;
        SetHealth(currentHealth - damage);

        float actualDamage = Mathf.Max(0f, previousHealth - currentHealth);
    }

    public virtual void SetHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        CheckHealthThreshold();
    }

    public virtual void CheckHealthThreshold()
    {
        if (currentHealth <= 0f)
        {
            //die
        };
    }

    protected virtual void DisableCollidersForDeath()
    {
        // Disable detection collider
        if (detectionCollider != null)
            detectionCollider.enabled = false;

        // Disable attack collider
        if (attackCollider != null)
            attackCollider.enabled = false;

        // Disable any other colliders on the main GameObject (used for lock-on)
        var mainCollider = GetComponent<Collider>();
        if (mainCollider != null && mainCollider != detectionCollider && mainCollider != attackCollider)
            mainCollider.enabled = false;
    }

    // Update is called once per frame
    public void EnableAttackHitbox()
    {
        isAttackBoxActive = true;
        if (attackCollider != null)
            attackCollider.enabled = true;
    }
    public void DisableAttackHitbox()
    {
        isAttackBoxActive = false;
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    protected virtual void OnDrawGizmos()
    {
        // Detection range gizmo (sphere)
        if (showDetectionGizmo)
        {
            float effectiveRange = GetEffectiveDetectionRange();
            Gizmos.color = new Color(0f, 0.7f, 1f, 0.3f); // Cyan, semi-transparent
            Gizmos.DrawWireSphere(transform.position, effectiveRange);
            Gizmos.color = new Color(0f, 0.7f, 1f, 0.1f);
            Gizmos.DrawSphere(transform.position, effectiveRange);
        }

        // Attack range gizmo (box)
        if (showAttackGizmo)
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f); // Red, semi-transparent
            Vector3 boxCenter = transform.position + transform.forward * attackBoxDistance;
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.1f);
            Gizmos.DrawCube(Vector3.zero, attackBoxSize);
            Gizmos.matrix = Matrix4x4.identity;
            // Also draw the effective attack range as a sphere for reference
            float attackRange = (Mathf.Max(attackBoxSize.x, attackBoxSize.z) * 0.5f) + attackBoxDistance;
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }

    protected float GetEffectiveDetectionRange()
    {
        if (detectionCollider != null)
            return GetScaledRadius(detectionCollider);

        return detectionRange;
    }
    private static float GetScaledRadius(SphereCollider collider)
    {
        if (collider == null)
            return 0f;

        Vector3 lossy = collider.transform.lossyScale;
        float maxScale = Mathf.Max(Mathf.Abs(lossy.x), Mathf.Abs(lossy.y), Mathf.Abs(lossy.z));
        if (maxScale <= 0f)
            return collider.radius;

        return collider.radius * maxScale;
    }
}


