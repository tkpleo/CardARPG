using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DeckBuilding.Cards;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using DeckBuilding;
using Player;

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
    private GameObject player = null;
    private AssignStartingDeck startingDeckAssigner;
    public List<CardInHand> cardsInHand = new List<CardInHand>();

    private void Start()
    {
        
        FindPlayerInScene();
        FindStartingDeckAssigner();
        AddDummyCardsToMasterDeck();
        // Draw the hand using DeckManager, then update the UI
        DeckManager.Instance.GetType().GetMethod("DrawHand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(DeckManager.Instance, null);

    }

    /// <summary>
    /// Adds two dummy cards to the master deck for UI testing.
    /// </summary>
    private void AddDummyCardsToMasterDeck()
    {
        if (DeckManager.Instance.MasterDeck.Count == 0)
        {
            var dummyCardData1 = ScriptableObject.CreateInstance<CardData>();
            SetDummyCardData(dummyCardData1, "Damage Boost", "Deal 2x damage.");

            Debug.Log($"Dummy card 1: {dummyCardData1.cardName}, {dummyCardData1.description}");

            var dummyCardData2 = ScriptableObject.CreateInstance<CardData>();
            SetDummyCardData(dummyCardData2, "Multi Strike", "Shoot Twice at Once.");

            Debug.Log($"Dummy card 2: {dummyCardData2.cardName}, {dummyCardData2.description}");

            DeckManager.Instance.MasterDeck.Add(new CardInstance(dummyCardData1));
            DeckManager.Instance.MasterDeck.Add(new CardInstance(dummyCardData2));
            RefreshHandUI(new CardInstance(dummyCardData1), new CardInstance(dummyCardData2));
        }
    }

    /// <summary>
    /// Helper to set all fields on dummy CardData.
    /// </summary>
    private void SetDummyCardData(CardData cardData, string cardName, string cardDescription)
    {
        // Try to set both field and property in case CardData uses either
        var type = cardData.GetType();
        var cardNameField = type.GetField("cardName");
        var cardNameProp = type.GetProperty("cardName");
        if (cardNameField != null) cardNameField.SetValue(cardData, cardName);
        if (cardNameProp != null && cardNameProp.CanWrite) cardNameProp.SetValue(cardData, cardName);

        var descField = type.GetField("description");
        var descProp = type.GetProperty("description");
        if (descField != null) descField.SetValue(cardData, cardDescription);
        if (descProp != null && descProp.CanWrite) descProp.SetValue(cardData, cardDescription);

        // Optionally set artwork to null or a test sprite
        var artField = type.GetField("artwork");
        var artProp = type.GetProperty("artwork");
        if (artField != null) artField.SetValue(cardData, null);
        if (artProp != null && artProp.CanWrite) artProp.SetValue(cardData, null);
    }

    /// <summary>
    /// Public method to refresh the hand UI. Call this after the hand changes.
    /// </summary>
    public void RefreshHandUI(CardInstance newLeftCard = null, CardInstance newRightCard = null)
    {
        // Clear old UI
        cardsInHand.Clear();
        if (leftHand != null)
        {
            leftHand.cardImage.sprite = null;
            leftHand.cardNameText.text = "";
            leftHand.cardDescriptionText.text = "";
        }
        if (rightHand != null)
        {
            rightHand.cardImage.sprite = null;
            rightHand.cardNameText.text = "";
            rightHand.cardDescriptionText.text = "";
        }
        // Load new hand
        CardInstance leftCard = newLeftCard ?? DeckManager.Instance.HandLeft;
        CardInstance rightCard = newRightCard ?? DeckManager.Instance.HandRight;
        LoadCardIntoHand(leftCard, true);
        LoadCardIntoHand(rightCard, false);
    }

    private void FindPlayerInScene()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player GameObject with tag 'Player' not found in the scene.");
            return;
        }
    }

    private void FindStartingDeckAssigner()
    {
        if (player == null)
        {
            Debug.LogWarning("Player GameObject is not assigned. Cannot find AssignStartingDeck component.");
            return;
        }

        startingDeckAssigner = player.GetComponent<AssignStartingDeck>();
        if (startingDeckAssigner == null)
        {
            Debug.LogWarning("AssignStartingDeck component not found on Player GameObject.");
        }
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
