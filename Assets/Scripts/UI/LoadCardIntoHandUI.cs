using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DeckBuilding.Cards;
using DeckBuilding;
using System;



public class LoadCardIntoHandUI : MonoBehaviour
{
    [Serializable]
    private struct HandUI
    {
        public Image cardImage;
        public TextMeshProUGUI cardNameText;
        public TextMeshProUGUI cardDescriptionText;
    }

    [Serializable]
    private struct DeckCounterUI
    {
        public TextMeshProUGUI countText;
    }

    #region Inspector Fields
    [SerializeField] private HandUI leftHand;
    [SerializeField] private HandUI rightHand;
    [SerializeField] private DeckCounterUI deckCounter;
    [SerializeField] private DeckCounterUI discardCounter;
    #endregion

    private void OnEnable() => DeckManager.OnDeckStateUpdate += RefreshHandUI;

    private void OnDisable() => DeckManager.OnDeckStateUpdate -= RefreshHandUI;
    

    /// <summary>
    /// Private method to refresh the hand UI. Call this after the hand changes.
    /// </summary>
    private void RefreshHandUI(DeckState deckState)
    {
        UpdateHandUI(deckState.HandLeft, leftHand);

        UpdateHandUI(deckState.HandRight, rightHand);

        deckCounter.countText.text = $"x{deckState.DrawPile.Count}";
        discardCounter.countText.text = $"x{deckState.DiscardPile.Count}";
    }

    private void UpdateHandUI(CardInstance card, HandUI uiElement)
    {
        uiElement.cardImage.sprite = card?.Artwork;
        uiElement.cardNameText.text = card?.Name;
        uiElement.cardDescriptionText.text = card?.Description;
    }
}
