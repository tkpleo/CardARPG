/*
 * Written by: Will T
 * 
 * The Deck Manager is a singleton resposible for managing the player's deck of cards. 
 * It keeps track of the player's master deck (the deck they build and modify outside of combat),
 * as well as the draw pile, discard pile, and hand during combat.
 * It purely focuses on the data and logic of the player's deck, 
 * and does not handle any visual or UI elements related to the deck.
 */

using UnityEngine;
using System.Collections.Generic;

namespace DeckBuilding
{
    using Cards;
    using System.Linq;

    public class DeckManager : Singleton<DeckManager>
    {
        /// <summary>
        /// The deck of the player outside of combat. 
        /// This is the deck that the player builds and modifies throughout the game.
        /// It is the deck that is used to generate the draw pile at the start of each combat.
        /// </summary>
        private List<CardInstance> masterDeck = new();

        /// <summary>
        /// The queue of cards that the player will draw from during combat.
        /// </summary>
        private List<CardInstance> drawPile = new();

        /// <summary>
        /// Discarded cards wait here until the draw pile refreshes.
        /// </summary>
        private List<CardInstance> discardPile = new();
        private CardInstance handLeft;
        private CardInstance handRight;

        #region General Combat Methods
        public static void StartCombat() => Instance.PrivStartCombat();

        // Private method to start combat
        private void PrivStartCombat()
        {
            // Assigns the draw pile to a new queue of card instances created from the master deck.
            drawPile = masterDeck
                .Select(data => new CardInstance(data))
                .ToList();

            Shuffle(drawPile);

            discardPile.Clear();

            DrawHand();
        }

        // private nonstatic method to end the combat
        public static void EndCombat() => Instance.PrivEndCombat();

        private void PrivEndCombat()
        {
            drawPile.Clear();
            discardPile.Clear();
        }
        #endregion

        #region Public Card Manipulation
        public static void PlayCard(bool isLeftHand, CardContext context) => Instance.PrivPlayCard(isLeftHand, context);
        private void PrivPlayCard(bool isLeftHand, CardContext context)
        {
            var card = isLeftHand ? handLeft : handRight;
            if (card == null) return;

            card.Play(context);
        }
        public static void ReshuffleDiscardIntoDraw() => Instance.PrivReshuffleDiscardIntoDraw();
        private void PrivReshuffleDiscardIntoDraw()
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
            Shuffle(drawPile);
        }
        #endregion

        private static void Shuffle(List<CardInstance> pile)
        {
            for (int i = pile.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (pile[i], pile[j]) = (pile[j], pile[i]);
            }
        }

        private void DrawHand()
        {
            for (int i = 0; i < 2; i++)
            {
                if (drawPile.Count == 0)
                {
                    if (discardPile.Count == 0)
                    {
                        // No cards left to draw
                        return;
                    }
                    ReshuffleDiscardIntoDraw();
                }

                var card = drawPile[^1]; // Get the last card in the draw pile
                drawPile.RemoveAt(drawPile.Count - 1); // Remove it from the draw pile

                // Add it to one of the slots in the player's hand.
                if (i == 0)
                    handLeft = card;
                else
                    handRight = card;
            }
        }

        public void AddCardsToMasterDeck(CardData[] cards)
        {
            foreach (var card in cards)
            {
                masterDeck.Add(new CardInstance(card));
            }
        }
    }
}
