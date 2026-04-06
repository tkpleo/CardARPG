/*
 * Written by: Will T
 * 
 * The PlayerComponent which controls the animator component on the player.
 * It listens to the other components on the player for changes and updates the animator accordingly.
 */

using Player.Attack;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : PlayerComponent
    {
        private Animator animator;
        private PlayerController movement;
        private PlayerAttack attack;

        protected override void Awake()
        {
            base.Awake();

            if (TryGetComponent<Animator>(out var anim)) animator = anim;
            else throw new System.Exception("Animator component not found on Player GameObject.");
        }

        private void Start()
        {
            if (Player.TryGetComponent(out PlayerController movement))
            {
                this.movement = movement;
            }      
        }

        private void FixedUpdate()
        {
            if (animator == null) return;

            UpdateMovementBooleans();
            UpdateAttackBooleans();
        }

        private void UpdateMovementBooleans()
        {
            if (movement == null) return;
            animator.SetBool("isRunning", movement.isMoving);
            animator.SetBool("isDodging", movement.isDodging);
        }

        private void UpdateAttackBooleans()
        {
            if (attack == null) return;
            animator.SetBool("isAttacking", attack.isAttacking);
        }
    }
}
