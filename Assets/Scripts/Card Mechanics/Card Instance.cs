/*
 * Written by: Will T
 * 
 * CardInstance is the class which represents a physical instance of a card within the game.
 * Your deck, hand, and discard pile will all be made up of specifically CardInstances, not CardData.
 * Each CardInstance holds a reference to the CardData which defines the card, as well as any relevant information about that specific instance of the card (for example, whether or not it is upgraded).
 * CardInstance also includes any functionality related to using or manipulating the physical card, like playing it or upgrading it.
 */

namespace DeckBuilding.Cards
{
    public class CardInstance
    {
        public CardData data;
        public bool isUpgraded;

        public CardInstance(CardData data, bool isUpgraded = false)
        {
            this.data = data;
            this.isUpgraded = isUpgraded;
        }

        public CardInstance(CardInstance other)
        {
            data = other.data;
            isUpgraded = other.isUpgraded;
        }

        public void Play(CardContext context)
        {
            foreach (var effect in data.effects)
            {
                effect.ExecuteEffect(context);
            }
        }
    }
}
