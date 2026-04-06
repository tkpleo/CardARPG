/*
 * Written by: Will T
 * 
 * This file defines the CardEffect abstract class and all implementations of it.
 * 
 * A CardEffect is meant to be a basic action a card could perform when played. For example, a card could shoot a bullet from the player's gun.
 * The CardData script holds a list CardEffects, and when the card is played, it executes each CardEffect in order.
 */

using UnityEngine;
using System.ComponentModel;

namespace DeckBuilding.Cards
{
    using Player;
    using Player.Attack;

    public abstract class CardEffect : ScriptableObject
    {
        public int value;

        public abstract void ExecuteEffect(CardContext context);
    }

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
