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

        public override void ExecuteEffect(CardContext context)
        {
            if (!Player.TryGetComponent(out PlayerAttack _PlayerAttack))
                throw new System.Exception("Player does not have a PlayerAttack component.");

            _PlayerAttack.InitAttack(bulletData); // Shoot the bullet specified by bulletData
        }
    }
}