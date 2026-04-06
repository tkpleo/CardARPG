/*
 * Written by: Will T
 * 
 * This class is purely responsible for handling the player shooting bullets.
 * It is not responsible for the player clicking or playing cards.
 * When a card is played that shoots a bullet, it should call this class to handle the shooting logic.
 */

using System.Collections;
using UnityEngine;

namespace Player.Attack
{
    public class PlayerAttack : PlayerComponent
    {
        [Header("Attacking Variables")]
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackTime = 0.2f;

        [Header("Bullet Variables")]
        [SerializeField]
        private GameObject bulletSpawnPoint;
        [SerializeField, LabelOverride("Default Bullet Type")]
        private BulletData bulletPrefab;

        public bool isAttacking { get; private set; }

        private PlayerController movement;

        private void Start()
        {
            if (Player.TryGetComponent(out PlayerController _movement)) movement = _movement;
             else Debug.LogWarning("PlayerController component not found on Player GameObject.");
        }

        private void OnEnable()
        {
            if (bulletPrefab != null) BulletPooler.Prewarm(bulletPrefab, 10);
            else Debug.LogWarning("Bullet prefab not assigned in PlayerAttack.");
        }

        public void InitAttack(BulletData bullet)
        {
            // Preliminary checks to avoid errors - ensure all necessary references are assigned
            if (bulletSpawnPoint == null) return;

            Ray ray = movement.mainCamera.ScreenPointToRay(movement.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo)) return; // No valid target hit, so exit early

            Vector3 lookAtPoint = hitInfo.point;
            Vector3 direction = lookAtPoint - transform.position;
            direction.y = 0;


            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            StartCoroutine(Attack());

            // Use provided bullet data or default prefab if none given
            BulletData bulletData = bullet ?? bulletPrefab;

            // Get a pooled bullet (inactive)
            BulletBehavior pooledBullet = BulletPooler.GetObject(bulletData);
            if (pooledBullet == null) return;

            // Position and rotate before activation so transform is correct when enabled
            pooledBullet.transform.SetPositionAndRotation(bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);

            // Activate (this triggers OnEnable in BulletBehavior to apply default BulletData)
            pooledBullet.gameObject.SetActive(true);
        }

        private IEnumerator Attack()
        {
            isAttacking = true;
            yield return new WaitForSeconds(attackTime);
            isAttacking = false;
            yield return new WaitForSeconds(attackCooldown);

        }


        /*
        

                
            */
    }
}
