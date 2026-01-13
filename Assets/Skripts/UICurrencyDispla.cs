using UnityEngine;
using TMPro;
using System; // Добавляем для использования Action

public class UICurrencyDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private ShopItem shopItem;

    [Header("Settings")]
    [SerializeField] private bool showCoinsBalance = true;
    [SerializeField] private bool showItemPrice = false;
    [SerializeField] private bool hidePriceWhenPurchased = true;
    [SerializeField] private bool hidePriceWhenSelected = true;

    void Update()
    {
        // Периодически проверяем состояние товара
        if (shopItem != null && showItemPrice)
        {
            // Проверяем, изменилось ли состояние с последней проверки
            bool currentPurchased = shopItem.IsPurchased;
            bool currentSelected = shopItem.IsSelected;

            if (currentPurchased != lastPurchasedState ||
                currentSelected != lastSelectedState)
            {
                UpdatePriceDisplay();
                lastPurchasedState = currentPurchased;
                lastSelectedState = currentSelected;
            }
        }
    }
    /*
    private void UnsubscribeFromEvents()
    {
        if (currencyManager != null)
        {
            currencyManager.OnCoinsChanged -= HandleCoinsChanged;
        }

        // Отписываемся от события изменения состояния товара
        if (shopItem != null)
        {
            shopItem.OnItemStateChanged -= HandleItemStateChanged;
        }
    }
    private void SubscribeToEvents()
    {
        if (currencyManager != null)
        {
            currencyManager.OnCoinsChanged += HandleCoinsChanged;
        }

        // Подписываемся на событие изменения состояния товара
        if (shopItem != null)
        {
            shopItem.OnItemStateChanged += HandleItemStateChanged;
        }
    }
    */
    private bool lastPurchasedState;
    private bool lastSelectedState;
    void Start()
    {
        InitializeReferences();
        
        UpdateDisplays();
    }

    

    private void InitializeReferences()
    {
        if (currencyManager == null)
            currencyManager = CurrencyManager.Instance;

        if (shopItem == null)
            shopItem = GetComponentInParent<ShopItem>();
    }

   

  

    private void HandleCoinsChanged(int newBalance)
    {
        UpdateCoinsDisplay(newBalance);
        UpdatePriceDisplay(); // Обновляем цену, так как баланс изменился
    }

    private void HandleItemStateChanged()
    {
        UpdatePriceDisplay(); // Обновляем при изменении состояния товара
    }

    private void UpdateDisplays()
    {
        if (currencyManager != null && showCoinsBalance)
        {
            UpdateCoinsDisplay(currencyManager.coins);
        }
        UpdatePriceDisplay();
    }

    private void UpdateCoinsDisplay(int coinsAmount)
    {
        if (coinsText != null && showCoinsBalance)
        {
            coinsText.text = coinsAmount.ToString();
        }
    }

    private void UpdatePriceDisplay()
    {
        if (priceText == null || !showItemPrice || shopItem == null) return;

        // Проверяем условия скрытия цены через публичные свойства
        bool shouldHidePrice =
            (hidePriceWhenPurchased && shopItem.IsPurchased) ||
            (hidePriceWhenSelected && shopItem.IsSelected);

        priceText.gameObject.SetActive(!shouldHidePrice);

        if (!shouldHidePrice)
        {
            priceText.text = shopItem.price.ToString();

            // Изменение цвета если не хватает денег
            if (currencyManager != null)
            {
                priceText.color = currencyManager.coins >= shopItem.price ?
                    Color.white :
                    new Color(1f, 0.3f, 0.3f);
            }
        }
    }

    // Для обновления цены извне
    public void SetPrice(int newPrice)
    {
        if (priceText != null && showItemPrice)
        {
            priceText.text = newPrice.ToString();
        }
    }
}