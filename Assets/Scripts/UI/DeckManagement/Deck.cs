using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour
{
    #region Fields and Properties
    public static Deck instance { get; private set; } //singleton

    [SerializeField] private CardCollection _playerDeck;
    [SerializeField] private Card _cardPrefab;

    [SerializeField] private Canvas _cardCanvas;


    public List<Card> _deckPile = new();
    public List<Card> _discardPile = new();

    public List<Card> HandCards { get; private set; } = new();

    #endregion

    #region Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InstantiateDeck();
    }

    private void InstantiateDeck()
    {
        for (int i = 0; i < _playerDeck.CardsInCollection.Count; i++)
        {
            Card card = Instantiate(_cardPrefab, _cardCanvas.transform);
            card.SetUp(_playerDeck.CardsInCollection[i]);
            _deckPile.Add(card);
            card.gameObject.SetActive(false);
        }

        ShuffleDeck();
    }

    private void ShuffleDeck() 
    {
        for (int i = _deckPile.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = _deckPile[i];
            _deckPile[i] = _deckPile[j];
            _deckPile[j] = temp;
        }
    }

    public void DrawHand(int amount = 5)
    {
        for (int i = 0; i < amount; i++)
        {
            if (_deckPile.Count <= 0)
            {
                if (_discardPile.Count > 0)
                {
                    // Move all discard cards back into the deck, then shuffle
                    _deckPile.AddRange(_discardPile);
                    _discardPile.Clear();
                    ShuffleDeck();
                }
                else
                {
                    // No cards available anywhere
                    break;
                }
            }

            if (_deckPile.Count > 0)
            {
                var card = _deckPile[0];
                HandCards.Add(card);
                card.gameObject.SetActive(true);
                _deckPile.RemoveAt(0);
            }
        }
    }

    //Removes a specific card from the hand and places it in the discard pile
    //called every time a card is played
    public void DiscardCard(Card card)
    {
        if (HandCards.Contains(card))
        {
            HandCards.Remove(card);
            _discardPile.Add(card);
            card.gameObject.transform.SetParent(_cardCanvas.transform, false);
            card.gameObject.SetActive(false);
        }
    }
    public void DiscardHand()
    {
        if (HandCards.Count == 0) return;

        // Move items safely by iterating backwards to avoid modifying the list while iterating
        for (int i = HandCards.Count - 1; i >= 0; i--)
        {
            var card = HandCards[i];
            HandCards.RemoveAt(i);
            _discardPile.Add(card);
            card.gameObject.transform.SetParent(_cardCanvas.transform, false);
            card.gameObject.SetActive(false);
        }
    }

    #endregion
}

