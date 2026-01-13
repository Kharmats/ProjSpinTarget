using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnStartClick);
    }

    private void OnStartClick()
    {
        Debug.Log("Start button clicked");

        // ¬сегда начинаем с первого уровн€
        int levelToLoad = 1;

        // ≈сли LevelManager инициализирован и есть прогресс
        if (LevelManager.Instance != null)
        {
            Debug.Log($"Current level: {LevelManager.Instance.currentLevel}");

            // «агружаем следующий уровень после последнего пройденного
            levelToLoad = LevelManager.Instance.currentLevel;

            // ƒл€ первого запуска загружаем уровень 1
            if (levelToLoad < 1) levelToLoad = 1;
        }

        Debug.Log($"Loading level: {levelToLoad}");
        LevelManager.Instance.LoadLevel(levelToLoad);
    }
}