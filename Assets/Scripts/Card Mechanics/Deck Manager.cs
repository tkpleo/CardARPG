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
using System;

namespace DeckBuilding
{
    using Cards;
    using System.Linq;
    
    /// <summary>
    /// A struct to represent the current state of the player's deck, including master deck, draw pile, discard pile, and hand.
    /// </summary>
    public struct DeckState
    {
        public List<CardInstance> MasterDeck;
        public List<CardInstance> DrawPile;
        public List<CardInstance> DiscardPile;
        public CardInstance HandLeft;
        public CardInstance HandRight;

        public DeckState(List<CardInstance> masterDeck, List<CardInstance> drawPile, List<CardInstance> discardPile, CardInstance handLeft, CardInstance handRight)
        {
            MasterDeck = masterDeck;
            DrawPile = drawPile;
            DiscardPile = discardPile;
            HandLeft = handLeft;
            HandRight = handRight;
        }
    }

    public class DeckManager : Singleton<DeckManager>
    {
        /// <summary>
        /// The deck of the player outside of combat. 
        /// This is the deck that the player builds and modifies throughout the game.
        /// It is the deck that is used to generate the draw pile at the start of each combat.
        /// </summary>
        private readonly List<CardInstance> MasterDeck = new();

        /// <summary>
        /// The queue of cards that the player will draw from during combat.
        /// </summary>
        private readonly List<CardInstance> DrawPile = new();

        /// <summary>
        /// Discarded cards wait here until the draw pile refreshes.
        /// </summary>
        private readonly List<CardInstance> DiscardPile = new();

        private CardInstance HandLeft;
        private CardInstance HandRight;

        /// <summary>
        /// Outputs the current state of the player's deck whenever it changes so other systems can react and update accordingly.
        /// </summary>
        public static event Action<DeckState> OnDeckStateUpdate;

        private void UpdateDeckState() => OnDeckStateUpdate?.Invoke(new DeckState(MasterDeck, DrawPile, DiscardPile, HandLeft, HandRight));

        #region General Combat Methods
        public static void StartCombat() => Instance.PrivStartCombat();

        // Private method to start combat
        private void PrivStartCombat()
        {
            // Assigns the draw pile to a new queue of card instances created from the master deck.
            DrawPile.Clear();
            DrawPile.AddRange(MasterDeck.Select(card => new CardInstance(card)));

            Shuffle(DrawPile);

            DiscardPile.Clear();

            DrawHand();

            UpdateDeckState();
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

            DrawHand();
        }

        public static void ReshuffleDiscardIntoDraw() => Instance.PrivReshuffleDiscardIntoDraw();
        private void PrivReshuffleDiscardIntoDraw()
        {
            Debug.Log("Reshuffling discard pile into draw pile...");

            DrawPile.AddRange(DiscardPile);
            DiscardPile.Clear();
            Shuffle(DrawPile);
        }
        #endregion

        private static void Shuffle(List<CardInstance> pile)
        {
            for (int i = pile.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (pile[i], pile[j]) = (pile[j], pile[i]);
            }
        }

        private void DrawHand()
        {
            if (HandLeft != null)
                DiscardPile.Add(HandLeft);

            if (HandRight != null)
                DiscardPile.Add(HandRight);

            // Draw two cards from the draw pile
            for (int i = 0; i < 2; i++)
            {
                bool left = i == 0;

                if (DrawPile.Count == 0)
                {
                    if (DiscardPile.Count == 0)
                    {
                        // No cards left to draw
                        Debug.Log("No cards left to draw!");
                        return;
                    }

                    ReshuffleDiscardIntoDraw();
                }

                var card = DrawPile[^1]; // Get the last card in the draw pile
                DrawPile.RemoveAt(DrawPile.Count - 1); // Remove it from the draw pile

                // Add it to one of the slots in the player's hand.
                if (left)
                    HandLeft = card;
                else
                    HandRight = card;
            }

            UpdateDeckState();
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
