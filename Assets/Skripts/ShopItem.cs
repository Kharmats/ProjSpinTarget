using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ShopItem : MonoBehaviour
{
    public enum ItemType { BallSkin, TargetSkin }

    public ItemType itemType;
    public int itemId;
    public int price;

    [Header("UI References")]
    public Button buyButton;
    public Button selectButton;
    public Button selectedButton;
    public Text priceText;

    [Header("Price Display Settings")]
    public bool hidePriceWhenPurchased = true;
    public bool hidePriceWhenSelected = true;

    private static List<ShopItem> allShopItems = new List<ShopItem>();

    private string PurchaseKey => $"Item_{itemType}_{itemId}_Purchased";

    public bool IsPurchased
    {
        get => PlayerPrefs.GetInt(PurchaseKey, 0) == 1;
        private set
        {
            PlayerPrefs.SetInt(PurchaseKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public bool IsSelected
    {
        get
        {
            if (itemType == ItemType.BallSkin)
                return itemId == PlayerPrefs.GetInt("SelectedBallSkin", -1);
            else
                return itemId == PlayerPrefs.GetInt("SelectedTargetSkin", -1);
        }
    }

    void Start()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuyItem);
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(SelectItem);
        }

        if (priceText != null)
        {
            priceText.text = price.ToString();
        }

        // Подписываемся на изменение баланса
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged += HandleCoinsChanged;
        }

        UpdateUI();
    }

    void OnEnable()
    {
        allShopItems.Add(this);
    }

    void OnDisable()
    {
        allShopItems.Remove(this);
    }

    private void HandleCoinsChanged(int newBalance)
    {
        UpdatePriceColor();
    }

    public void BuyItem()
    {
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.SpendCoins(price))
        {
            IsPurchased = true;
            UpdateUI();
            SelectItem();
            UpdateAllShopItemsUI(); // Обновляем все товары после покупки
        }
    }

    public void SelectItem()
    {
        if (!IsPurchased) return;

        if (GameShopManager.Instance != null)
        {
            GameShopManager.Instance.SelectItem(this);
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        // Обновление кнопок
        if (buyButton != null)
        {
            buyButton.gameObject.SetActive(!IsPurchased);
            buyButton.interactable = CurrencyManager.Instance != null &&
                                    CurrencyManager.Instance.coins >= price;
        }

        if (selectButton != null)
        {
            selectButton.gameObject.SetActive(IsPurchased && !IsSelected);
        }

        if (selectedButton != null)
        {
            selectedButton.gameObject.SetActive(IsPurchased && IsSelected);
        }

        // Обновляем текст цены
        if (priceText != null)
        {
            bool shouldHidePrice = (hidePriceWhenPurchased && IsPurchased) ||
                                  (hidePriceWhenSelected && IsSelected);

            priceText.gameObject.SetActive(!shouldHidePrice);

            if (!shouldHidePrice)
            {
                UpdatePriceColor();
            }
        }

        // Визуальное выделение
        Image background = GetComponent<Image>();
        if (background != null)
        {
            background.color = IsSelected ? new Color(0.8f, 1f, 0.8f) : Color.white;
        }
    }

    private void UpdatePriceColor()
    {
        if (priceText != null && priceText.gameObject.activeSelf)
        {
            if (CurrencyManager.Instance != null)
            {
                priceText.color = CurrencyManager.Instance.coins >= price ?
                    Color.white :
                    new Color(1f, 0.3f, 0.3f);
            }
        }
    }

    public static void UpdateAllShopItemsUI()
    {
        foreach (var item in allShopItems)
        {
            if (item != null)
            {
                item.UpdateUI();
            }
        }
    }
}