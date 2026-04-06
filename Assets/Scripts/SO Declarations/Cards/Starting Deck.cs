using UnityEngine;

namespace DeckBuilding.Cards
{
    [CreateAssetMenu(fileName = "StartingDeck", menuName = "Cards/Starting Deck")]
    public class StartingDeck : ScriptableObject
    {
        public string deckName;

        public CardData[] startingCards;
    }
}
