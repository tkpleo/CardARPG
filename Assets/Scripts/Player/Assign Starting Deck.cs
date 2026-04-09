/*
 * Written by: Will T
 * 
 * Simple component that can add a starting deck to the player and start combat.
 * Not designed to be used in the complete game.
 */

using Player;
using UnityEngine;
using DeckBuilding.Cards;
using DeckBuilding;

namespace Player
{
    public class AssignStartingDeck : PlayerComponent
    {
        [SerializeField] private StartingDeck initialDeck;
    private void Start()
        {
            DeckManager.Instance.AddCardsToMasterDeck(initialDeck.startingCards);

            DeckManager.StartCombat();
        }
    }
}
