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
        public List<CardInstance> MasterDeck { get; private set; } = new();

        /// <summary>
        /// The queue of cards that the player will draw from during combat.
        /// </summary>
        public List<CardInstance> DrawPile { get; private set; } = new();

        /// <summary>
        /// Discarded cards wait here until the draw pile refreshes.
        /// </summary>
        public List<CardInstance> DiscardPile { get; private set; } = new();

        public CardInstance HandLeft { get; private set; }
        public CardInstance HandRight { get; private set; }

        #region General Combat Methods
        public static void StartCombat() => Instance.PrivStartCombat();

        // Private method to start combat
        private void PrivStartCombat()
        {
            // Assigns the draw pile to a new queue of card instances created from the master deck.
            DrawPile = MasterDeck
                .Select(data => new CardInstance(data))
                .ToList();

            Shuffle(DrawPile);

            DiscardPile.Clear();

            DrawHand();
        }

        // private nonstatic method to end the combat
        public static void EndCombat() => Instance.PrivEndCombat();

        private void PrivEndCombat()
        {
            DrawPile.Clear();
            DiscardPile.Clear();
        }
        #endregion

        #region Public Card Manipulation
        public static void PlayCard(bool isLeftHand, CardContext context) => Instance.PrivPlayCard(isLeftHand, context);
        private void PrivPlayCard(bool isLeftHand, CardContext context)
        {
            var card = isLeftHand ? HandLeft : HandRight;
            if (card == null) return;

            card.Play(context);
        }
        public static void ReshuffleDiscardIntoDraw() => Instance.PrivReshuffleDiscardIntoDraw();
        private void PrivReshuffleDiscardIntoDraw()
        {
            DrawPile.AddRange(DiscardPile);
            DiscardPile.Clear();
            Shuffle(DrawPile);
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

        public void PublicDrawHand() => DrawHand();

        private void DrawHand()
        {
            for (int i = 0; i < 2; i++)
            {
                if (DrawPile.Count == 0)
                {
                    if (DiscardPile.Count == 0)
                    {
                        // No cards left to draw
                        return;
                    }
                    ReshuffleDiscardIntoDraw();
                }

                var card = DrawPile[^1]; // Get the last card in the draw pile
                DrawPile.RemoveAt(DrawPile.Count - 1); // Remove it from the draw pile

                // Add it to one of the slots in the player's hand.
                if (i == 0)
                    HandLeft = card;
                else
                    HandRight = card;
            }
        }

        public void AddCardsToMasterDeck(CardData[] cards)
        {
            foreach (var card in cards)
            {
                MasterDeck.Add(new CardInstance(card));
            }
        }
    }
}
