using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StatsDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text winsText;
    public TMP_Text averageTempoText;
    public TMP_Text lastGameTempoText;

    void OnEnable()
    {
        RefreshStats();
    }

    public void RefreshStats()
    {
        // Попробуем найти существующий GameStatsManager
        GameStatsManager statsManager = FindObjectOfType<GameStatsManager>();

        if (statsManager != null)
        {
            statsManager.LoadStats();
            UpdateUI(statsManager);
        }
        else
        {
            // Если не найден, создаем временный экземпляр
            Debug.LogWarning("GameStatsManager not found, creating temporary instance");
            GameObject tempObj = new GameObject("TempStatsManager");
            GameStatsManager tempManager = tempObj.AddComponent<GameStatsManager>();
            tempManager.LoadStats();
            UpdateUI(tempManager);
            Destroy(tempObj);
        }
    }

    private void UpdateUI(GameStatsManager statsManager)
    {
        winsText.text = $"{statsManager.TotalWins}";
        averageTempoText.text = $"{statsManager.AverageTapTempo:F2}";
        lastGameTempoText.text = $"{statsManager.LastGameTempo:F2}";

        Debug.Log($"Stats displayed: Wins={statsManager.TotalWins}, " +
                 $"AvgTempo={statsManager.AverageTapTempo:F2}, " +
                 $"LastTempo={statsManager.LastGameTempo:F2}");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}