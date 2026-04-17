/*
 * Written by: Will T
 * 
 * CardInstance is the class which represents a physical instance of a card within the game.
 * Your deck, hand, and discard pile will all be made up of specifically CardInstances, not CardData.
 * Each CardInstance holds a reference to the CardData which defines the card, as well as any relevant information about that specific instance of the card (for example, whether or not it is upgraded).
 * CardInstance also includes any functionality related to using or manipulating the physical card, like playing it or upgrading it.
 */

using UnityEngine;

namespace DeckBuilding.Cards
{
    public class CardInstance
    {
        public CardData data;

        public string Name => data.cardName;
        public string Description => data.description;
        public Sprite Artwork => data.artwork;

        public CardInstance(CardData data) => this.data = data;

        public CardInstance(CardInstance other) => this.data = other.data;

        public void Play(CardContext context)
        {
            foreach (var effect in data.effects)
            {
                effect.ExecuteEffect(context);
            }
        }
    }
}
