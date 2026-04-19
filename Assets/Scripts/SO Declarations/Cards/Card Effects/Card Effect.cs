/*
 * Written by: Will T
 * 
 * This file defines the CardEffect abstract class and all implementations of it.
 * 
 * A CardEffect is meant to be a basic action a card could perform when played. For example, a card could shoot a bullet from the player's gun.
 * The CardData script holds a list CardEffects, and when the card is played, it executes each CardEffect in order.
 */

using UnityEngine;

namespace DeckBuilding.Cards
{
    public abstract class CardEffect : ScriptableObject
    {
        public int value;

        public abstract void ExecuteEffect(CardContext context);
    }
}
