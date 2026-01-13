using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameShopManager : MonoBehaviour
{
    public static GameShopManager Instance;

    public List<ShopItem> ballSkins = new List<ShopItem>();
    public List<ShopItem> targetSkins = new List<ShopItem>();

    private const string FIRST_LAUNCH_KEY = "FirstLaunch";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeDefaultSkins();
        CheckFirstLaunch();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySelectedSkins();
    }

    void Start()
    {
        ApplySelectedSkins();
    }

    private void CheckFirstLaunch()
    {
        if (!PlayerPrefs.HasKey(FIRST_LAUNCH_KEY))
        {
            PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 1);
            PlayerPrefs.Save();

            // При первом запуске выбираем стандартные скины
            SelectDefaultSkins();
        }
    }

    private void SelectDefaultSkins()
    {
        // Устанавливаем стандартные скины как выбранные
        PlayerPrefs.SetInt("SelectedBallSkin", 4);
        PlayerPrefs.SetInt("SelectedTargetSkin", 1);
        PlayerPrefs.Save();
    }

    private void InitializeDefaultSkins()
    {
        // Инициализация PlayerPrefs для стандартных скинов
        if (!PlayerPrefs.HasKey("SelectedBallSkin"))
        {
            PlayerPrefs.SetInt("SelectedBallSkin", 4);
        }

        if (!PlayerPrefs.HasKey("SelectedTargetSkin"))
        {
            PlayerPrefs.SetInt("SelectedTargetSkin", 1);
        }

        // Установка стандартных скинов как купленных
        string defaultBallKey = $"Item_{ShopItem.ItemType.BallSkin}_4_Purchased";
        if (!PlayerPrefs.HasKey(defaultBallKey))
        {
            PlayerPrefs.SetInt(defaultBallKey, 1);
        }

        string defaultTargetKey = $"Item_{ShopItem.ItemType.TargetSkin}_1_Purchased";
        if (!PlayerPrefs.HasKey(defaultTargetKey))
        {
            PlayerPrefs.SetInt(defaultTargetKey, 1);
        }

        PlayerPrefs.Save();
    }

    private void ApplySelectedSkins()
    {
        // Находим выбранные скины и обновляем их UI
        ShopItem selectedBall = FindSkinById(ballSkins, PlayerPrefs.GetInt("SelectedBallSkin", 4));
        ShopItem selectedTarget = FindSkinById(targetSkins, PlayerPrefs.GetInt("SelectedTargetSkin", 1));

        // Обновляем UI
        RefreshAllItems();
    }

    private ShopItem FindSkinById(List<ShopItem> skins, int id)
    {
        foreach (ShopItem item in skins)
        {
            if (item != null && item.itemId == id)
            {
                return item;
            }
        }
        return null;
    }

    public void SelectItem(ShopItem selectedItem)
    {
        if (selectedItem == null) return;

        // Сохраняем выбор в PlayerPrefs
        if (selectedItem.itemType == ShopItem.ItemType.BallSkin)
        {
            PlayerPrefs.SetInt("SelectedBallSkin", selectedItem.itemId);
        }
        else
        {
            PlayerPrefs.SetInt("SelectedTargetSkin", selectedItem.itemId);
        }

        PlayerPrefs.Save();
        RefreshAllItems();
    }

    public void RefreshAllItems()
    {
        // Обновляем все мячи
        foreach (var item in ballSkins)
        {
            if (item != null)
            {
                item.UpdateUI();
            }
        }

        // Обновляем все мишени
        foreach (var item in targetSkins)
        {
            if (item != null)
            {
                item.UpdateUI();
            }
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}