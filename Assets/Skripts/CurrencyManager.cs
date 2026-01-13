using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int coins;
    public const string COINS_KEY = "PlayerCoins";

    [SerializeField] private Text coinText; // Ссылка на UI-текст для отображения монет

    private List<TextMeshProUGUI> coinsTexts = new List<TextMeshProUGUI>();

    public event Action<int> OnCoinsChanged; // Новое событие с передачей нового баланса

    public void RegisterCoinsText(TextMeshProUGUI text)
    {
        if (!coinsTexts.Contains(text))
        {
            coinsTexts.Add(text);
            text.text = coins.ToString(); // Инициализация текущим значением
        }
    }
    public void UnregisterCoinsText(TextMeshProUGUI text)
    {
        if (coinsTexts.Contains(text))
        {
            coinsTexts.Remove(text);
        }
    }

    private void UpdateAllCoinsTexts()
    {
        foreach (var text in coinsTexts)
        {
            if (text != null)
            {
                text.text = coins.ToString();
            }
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Новый метод для обновления текста
    private void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        UpdateAllCoinsTexts(); // Обновляем все текстовые элементы
    }

    public bool SpendCoins(int amount)
    {
        if (coins < amount) return false;

        coins -= amount;
        SaveCoins();
        UpdateAllCoinsTexts(); // Обновляем все текстовые элементы
        return true;
    }


    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COINS_KEY, coins);
        PlayerPrefs.Save();
    }

    public void LoadCoins()
    {
        coins = PlayerPrefs.GetInt(COINS_KEY, 0);
        UpdateCoinText(); // Обновляем UI после загрузки
    }

    // Для безопасного обновления текста при смене сцен
    public void SetCoinTextReference(Text newText)
    {
        coinText = newText;
        UpdateCoinText();
    }
}