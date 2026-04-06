/* 
 * Written by: Will T
 * 
 * This script defines the CardData scriptable object.
 * The scriptable object defines a card in the game.
 * It holds the data for the card, such as its name, description, artwork, and list of effects.
 * Each unique card in the game should have its own CardData asset in the editor.
 */

using System.Collections.Generic;
using UnityEngine;

namespace DeckBuilding.Cards
{
    [CreateAssetMenu(fileName = "Card Data", menuName = "Cards/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Generic Card Info")]
        public string cardName;
        [TextArea]
        public string description;
        public Sprite artwork;

        [Header("Card Mechanics")]
        public List<CardEffect> effects = new();
    }
}
