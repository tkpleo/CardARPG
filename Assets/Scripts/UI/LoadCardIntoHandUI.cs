using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DeckBuilding.Cards;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using DeckBuilding;
using Player;
using UnityEngine.InputSystem;

[System.Serializable]
public class HandUI
{
    public Image cardImage;
    private Sprite sprite;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDescriptionText;
}

[System.Serializable]
public class CardInHand
{
    public CardInstance card;
    public bool isLeftHand;
    public Image cardImage;
    internal Sprite CardSprite;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDescriptionText;
}
public class LoadCardIntoHandUI : MonoBehaviour
{
    public HandUI leftHand;
    public HandUI rightHand;
    public List<CardInHand> cardsInHand = new List<CardInHand>();

    [SerializeField] private InputActionReference leftCardAction;
    [SerializeField] private InputActionReference rightCardAction;
    [SerializeField] private InputActionReference reloadAction;

    private void OnEnable()
    {
        DeckManager.Instance.OnDeckStateUpdate += RefreshHandUI;
    }

    private void OnDisable()
    {
        DeckManager.Instance.OnDeckStateUpdate -= RefreshHandUI;
    }

    /// <summary>
    /// Public method to refresh the hand UI. Call this after the hand changes.
    /// </summary>
    public void RefreshHandUI(DeckState deckState)
    {
        LoadCardIntoHand(deckState.HandLeft, true);
        LoadCardIntoHand(deckState.HandRight, false);
    }


    private void LoadCardIntoHand(CardInstance card, bool isLeftHand)
    {
        if ((isLeftHand && leftHand == null) || (!isLeftHand && rightHand == null))
        {
            Debug.LogWarning($"{(isLeftHand ? "leftHand" : "rightHand")} is not assigned in the Inspector.");
            return;
        }

        Image selectedCardImage = isLeftHand ? leftHand.cardImage : rightHand.cardImage;
        TextMeshProUGUI selectedNameText = isLeftHand ? leftHand.cardNameText : rightHand.cardNameText;
        TextMeshProUGUI selectedDescriptionText = isLeftHand ? leftHand.cardDescriptionText : rightHand.cardDescriptionText;

        if (selectedCardImage == null || selectedNameText == null || selectedDescriptionText == null)
        {
            Debug.LogWarning($"One or more UI components are not assigned for {(isLeftHand ? "leftHand" : "rightHand")}.\ncardImage: {selectedCardImage != null}, cardNameText: {selectedNameText != null}, cardDescriptionText: {selectedDescriptionText != null}");
            return;
        }

        if (card == null)
        {
            Debug.LogWarning($"CardInstance is null for {(isLeftHand ? "leftHand" : "rightHand")}");
            return;
        }

        CardInHand cardInHand = new CardInHand
        {
            card = card,
            isLeftHand = isLeftHand,
            cardImage = selectedCardImage,
            CardSprite = card.Artwork,
            cardNameText = selectedNameText,
            cardDescriptionText = selectedDescriptionText
        };

        cardsInHand.Add(cardInHand);
        UpdateHandUI(cardInHand);
    }

    private void UpdateHandUI(CardInHand cardInHand)
    {
        if (cardInHand == null)
        {
            Debug.LogWarning("cardInHand is null in UpdateHandUI");
            return;
        }
        if (cardInHand.card == null)
        {
            Debug.LogWarning("cardInHand.card is null in UpdateHandUI");
            return;
        }
        Debug.Log($"Updating UI: {cardInHand.card.Name}, {cardInHand.card.Description}");
        if (cardInHand.isLeftHand)
        {
            if (leftHand == null || leftHand.cardImage == null || leftHand.cardNameText == null || leftHand.cardDescriptionText == null)
            {
                Debug.LogWarning("One or more leftHand UI components are not assigned in UpdateHandUI");
                return;
            }
            leftHand.cardImage.sprite = cardInHand.CardSprite;
            leftHand.cardNameText.text = cardInHand.card.Name;
            leftHand.cardDescriptionText.text = cardInHand.card.Description;
        }
        else
        {
            if (rightHand == null || rightHand.cardImage == null || rightHand.cardNameText == null || rightHand.cardDescriptionText == null)
            {
                Debug.LogWarning("One or more rightHand UI components are not assigned in UpdateHandUI");
                return;
            }
            rightHand.cardImage.sprite = cardInHand.CardSprite;
            rightHand.cardNameText.text = cardInHand.card.Name;
            rightHand.cardDescriptionText.text = cardInHand.card.Description;
        }
    }
}
