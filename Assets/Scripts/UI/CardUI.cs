using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    #region Fields and Properties

    private Card _card;

    [Header("Prefab Elements")]
    [SerializeField] private Image _cardImage;

    [SerializeField] private TextMeshProUGUI _cardName;
    [SerializeField] private TextMeshProUGUI _cardDescription;

    //[Header("Sprite Assets")]
    #endregion

    #region Methods

    private void Awake()
    {
        _card = GetComponent<Card>();
        SetCardUI();
    }

    private void OnValidate()
    {
        Awake();
    }

    public void SetCardUI()
    {
        if (_card != null && _card.cardData != null)
        {
            SetCardTexts();
            //SetCardImage();
        }

    }

    private void SetCardTexts()
    {
        _cardName.text = _card.cardData.CardName;
        _cardDescription.text = _card.cardData.CardDescription;
    }

    private void SetCardImage()
    {
        _cardImage.sprite = _card.cardData.Image;
    }

    #endregion
}
