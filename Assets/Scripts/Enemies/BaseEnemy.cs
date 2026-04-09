using UnityEngine;
using Stateless;
using UnityEngine.AI;


public abstract class BaseEnemy<TState, TTrigger> : BaseEnemyCore
        where TState : System.Enum
        where TTrigger : System.Enum
{
    protected StateMachine<TState, TTrigger> stateMachine;

    [Header("Key Enemy Components")]
    [SerializeField] public NavMeshAgent navMeshAgent;
    [SerializeField] protected Animator animator;

    [Header("Combat Colliders")]
    [SerializeField] protected Collider attackCollider;
    [SerializeField] protected Collider blockCollider;
    [SerializeField] protected Collider hitCollider;

    [Header("Life Management")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float currentHealth = 100f;

    [Header("Attack Management")]
    [SerializeField, Tooltip("The amount of damage this enemy deals per attack.")] public float attackDamage;
    [SerializeField, Tooltip("The range within which this enemy can attack.")] public float attackRange;
    [SerializeField, Tooltip("The cooldown period (in seconds) between attacks.")] public float attackCooldown;
    [SerializeField, Tooltip("How long the attack hitbox is active.")] public float attackDuration;

    [Header("Defense Management")]
    [SerializeField, Tooltip("The amount of damage this enemy can block per attack.")] public float blockAmount;
    [SerializeField, Tooltip("The cooldown period (in seconds) between blocks.")] public float blockCooldown;

    private Transform playerTransform;

    protected virtual void Awake()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    protected void InitializeStateMachine(TState initialState)
    {
        stateMachine = new StateMachine<TState, TTrigger>(initialState);
        
    }

    // For later: add specific configuration for each collider
    private void ConfirmEnemyColliders()
    {
        if (attackCollider == null)
            Debug.LogWarning($"{this.name} is missing an attack collider reference.");
        if (blockCollider == null)
            Debug.LogWarning($"{this.name} is missing a block collider reference.");
        if (hitCollider == null)
            Debug.LogWarning($"{this.name} is missing a hit collider reference.");
    }

    private void ConfirmPlayerReference()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            else
                Debug.LogWarning($"{this.name} could not find a GameObject with the 'Player' tag.");
        }
    }
}