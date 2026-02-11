using UnityEngine;

public class HandManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject Card1;
    private GameObject Card2;
    private GameObject Card3;
    private GameObject Card4;
    private GameObject Card5;

    public GameObject cardSlot1;
    public GameObject cardSlot2;
    public GameObject cardSlot3;
    public GameObject cardSlot4;
    public GameObject cardSlot5;

    private Canvas _cardCanvas;
    private BulletAbilityManager bulletAbilityManager;
    private Deck deck;

    public GameObject playerDeck;
    public GameObject Player;
    private int cardIndex;
    private int buffCount;
    public GameObject maxBuffsVFX;
    public GameObject goldenGunVFX;

    private void Start()
    {
        deck = playerDeck.GetComponent<Deck>();
        bulletAbilityManager = Player.GetComponent<BulletAbilityManager>();
        cardIndex = 0;
        deck.DrawHand(5);
        CardPlacement();
        buffCount = 0;
    }

    private void ResetBuffCount() 
    {
        buffCount = 0;
        if (maxBuffsVFX != null)
        {
            maxBuffsVFX.SetActive(false);
        }
    }

    public void RedrawIfBuffed() 
    { 
        if (buffCount > 0) 
        {
            RedrawHand();
            ResetBuffCount();
        }
    }

    public void CardPlacement() 
    {
        if (deck == null)
        {
            Debug.LogError("Deck is null in CardPlacement");
            return;
        }

        if (deck.HandCards == null)
        {
            Debug.LogWarning("HandCards is null when placing cards. Make sure DrawHand() populated the hand before calling CardPlacement().");
        }

        int count = deck.HandCards != null ? deck.HandCards.Count : 0;

        // Assign or clear each card slot safely
        if (count > 0 && deck.HandCards[0] != null) Card1 = deck.HandCards[0].gameObject;
        if (count > 1 && deck.HandCards[1] != null) Card2 = deck.HandCards[1].gameObject;
        if (count > 2 && deck.HandCards[2] != null) Card3 = deck.HandCards[2].gameObject;
        if (count > 3 && deck.HandCards[3] != null) Card4 = deck.HandCards[3].gameObject;
        if (count > 4 && deck.HandCards[4] != null) Card5 = deck.HandCards[4].gameObject;

        // Parent the assigned cards to their slots
        if (Card1 != null) Card1.transform.SetParent(cardSlot1.transform, false);
        if (Card2 != null) Card2.transform.SetParent(cardSlot2.transform, false);
        if (Card3 != null) Card3.transform.SetParent(cardSlot3.transform, false);
        if (Card4 != null) Card4.transform.SetParent(cardSlot4.transform, false);
        if (Card5 != null) Card5.transform.SetParent(cardSlot5.transform, false);
    }

    public void GetCardIndex(int card)
    {
        cardIndex = -1;
        switch (card)
        {
            case 1:
                if (Card1 != null) cardIndex = Card1.GetComponent<Card>().cardData.cardIndex;
                break;
            case 2:
                if (Card2 != null) cardIndex = Card2.GetComponent<Card>().cardData.cardIndex;
                break;
            case 3:
                if (Card3 != null) cardIndex = Card3.GetComponent<Card>().cardData.cardIndex;
                break;
            case 4:
                if (Card4 != null) cardIndex = Card4.GetComponent<Card>().cardData.cardIndex;
                break;
            case 5:
                if (Card5 != null) cardIndex = Card5.GetComponent<Card>().cardData.cardIndex;
                break;
            default:
                Debug.Log("Invalid card number");
                break;
        }

        if (cardIndex < 0)
            Debug.LogWarning($"Failed to get card index for card slot {card}. The slot may be empty.");
    }
    
    public void PlayCard(int card)
    {
        if (buffCount >= 3)
        {
            Debug.Log("Cannot apply more buffs, must fire bullet.");
            return;
        }
        if (card < 1 || card > 5)
        {
            Debug.LogWarning("PlayCard called with invalid card number: " + card);
            return;
        }

        GetCardIndex(card);

        if (cardIndex < 0)
        {
            Debug.LogWarning("No valid cardIndex found, aborting PlayCard.");
            return;
        }

        switch (cardIndex) 
        {
            case 1:
                bulletAbilityManager.BA_DamageIncrease += 1;
                //Debug.Log("Damage Increase applied");
                break;
            case 2:
                bulletAbilityManager.BA_DamageMultiplier += 1f;
                break;
            case 3:
                bulletAbilityManager.BA_Slow = true;
                break;
            case 4:
                bulletAbilityManager.BA_Stun = true;
                break;
            case 5:
                bulletAbilityManager.BA_AOE = true;
                break;
            case 6:
                bulletAbilityManager.BA_Opportunist = true;
                break;
            case 7:
                bulletAbilityManager.BA_GoldenGun = true;
                if (goldenGunVFX != null)
                {
                    goldenGunVFX.SetActive(true);
                }
                break;
            default:
                Debug.Log("Invalid card index");
                break;
        }

        // Resolve the selected Card object from the UI slot (safer than indexing HandCards directly)
        Card selectedCard = null;
        switch (card)
        {
            case 1: if (Card1 != null) selectedCard = Card1.GetComponent<Card>(); break;
            case 2: if (Card2 != null) selectedCard = Card2.GetComponent<Card>(); break;
            case 3: if (Card3 != null) selectedCard = Card3.GetComponent<Card>(); break;
            case 4: if (Card4 != null) selectedCard = Card4.GetComponent<Card>(); break;
            case 5: if (Card5 != null) selectedCard = Card5.GetComponent<Card>(); break;
        }

        if (selectedCard == null)
        {
            Debug.LogWarning("Selected card is null when trying to discard — possible race condition or slot is empty.");
            return;
        }

        deck.DiscardCard(selectedCard);
        buffCount += 1;
        if  (buffCount >= 3 && maxBuffsVFX != null)
        {
            maxBuffsVFX.SetActive(true);
        }
    }

    public void RedrawHand()
    {
        deck.DiscardHand();
        deck.DrawHand(5);
        CardPlacement();
    }
}
