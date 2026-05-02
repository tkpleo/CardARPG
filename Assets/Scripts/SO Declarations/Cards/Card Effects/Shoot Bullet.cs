/*
 * Written by: Will T
 * 
 * The Card Effect for shooting a bullet.
 * Has an input to determine what kind of bullet should be shot when the card effect is triggered
 */

namespace DeckBuilding.Cards
{
    using Player;
    using Player.Attack;
    using UnityEngine;
    using System.ComponentModel;

    [CreateAssetMenu(fileName = "Card Effect", menuName = "Cards/Effects/ShootBullet")]
    public class ShootBullet : CardEffect
    {
        [Description("The data of the bullet to shoot.")]
        public BulletData bulletData;
        [Description("The deviation of degrees for the bullet's accuracy.")]
        public float accuracy = 0;
        [Description("The number of bullets to shoot in a single attack.")]
        public int bulletCount = 1;

        public override void ExecuteEffect(CardContext context)
        {
            if (!Player.TryGetComponent(out PlayerAttack _PlayerAttack))
                throw new System.Exception("Player does not have a PlayerAttack component.");

            _PlayerAttack.InitAttack(bulletData, accuracy, bulletCount); // Shoot the bullet specified by bulletData with the specified accuracy
        }
    }
}