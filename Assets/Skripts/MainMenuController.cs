using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button startButton;
    public Button levelsButton;
    private int maxLevel = 10; // Укажите здесь реальное количество уровней

    void Start()
    {
        // Определяем максимальный доступный уровень
        maxLevel = FindMaxAvailableLevel();

        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButton);
        }
        if (levelsButton != null)
        {
            levelsButton.onClick.AddListener(OnLevelsButton);
        }
    }

    void OnStartButton()
    {
        int lastLevel = PlayerPrefs.GetInt("LastLevel", 1);

        // Корректируем уровень, если он превышает максимальный
        if (lastLevel > maxLevel)
        {
            lastLevel = maxLevel;
        }

        PlayerPrefs.SetInt("CurrentLevel", lastLevel);
        PlayerPrefs.Save();
        SceneManager.LoadScene($"GameScene{lastLevel}");
    }

    void OnLevelsButton()
    {
        SceneManager.LoadScene("LevelsScene");
    }

    // Метод для определения максимального уровня
    private int FindMaxAvailableLevel()
    {
        int max = 1;

        // Проверяем существование сцен по порядку
        for (int i = 1; i <= 100; i++) // 100 - верхняя граница проверки
        {
            string sceneName = $"GameScene{i}";
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                max = i;
            }
            else
            {
                // Прерываем цикл при первой недоступной сцене
                break;
            }
        }
        return max;
    }
}