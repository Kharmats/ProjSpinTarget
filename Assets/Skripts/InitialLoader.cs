using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialLoader : MonoBehaviour
{
    void Awake()
    {
        if (GameStatsManager.Instance == null)
        {
            GameObject statsManager = new GameObject("GameStatsManager");
            statsManager.AddComponent<GameStatsManager>();
            DontDestroyOnLoad(statsManager);
        }

        if (LevelManager.Instance == null)
        {
            GameObject levelManager = new GameObject("LevelManager");
            levelManager.AddComponent<LevelManager>();
            DontDestroyOnLoad(levelManager);
        }

        if (CurrencyManager.Instance == null)
        {
            GameObject currencyManager = new GameObject("CurrencyManager");
            currencyManager.AddComponent<CurrencyManager>();
            DontDestroyOnLoad(currencyManager);
        }

        if (GameShopManager.Instance == null)
        {
            GameObject shopManager = new GameObject("ShopManager");
            shopManager.AddComponent<GameShopManager>();
            DontDestroyOnLoad(shopManager);
        }

        if (GameSettings.Instance == null)
        {
            GameObject gameSettings = new GameObject("GameSettings");
            gameSettings.AddComponent<GameSettings>();
            DontDestroyOnLoad(gameSettings);
        }

        SceneManager.LoadScene("MainMenu");
    }
}