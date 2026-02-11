using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "CardCollection")]
public class CardCollection : ScriptableObject
{
    [field: SerializeField] public List<ScriptableCard> CardsInCollection { get; private set; }

    public void RemoveCardFromCollection(ScriptableCard card)
    {
        if (CardsInCollection.Contains(card))
        {
            CardsInCollection.Remove(card);
        }
        else
        {
            Debug.LogWarning("CardData is not present in collectoin");
        }
    }

    public void AddCardToCollection(ScriptableCard card)
    {
        CardsInCollection.Add(card);
    }
}
