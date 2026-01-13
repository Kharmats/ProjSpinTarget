using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance;

    private const string TOTAL_WINS_KEY = "TotalWins";
    private const string TOTAL_TAP_TEMPO_KEY = "TotalTapTempo";
    private const string GAMES_PLAYED_KEY = "GamesPlayed";
    private const string LAST_GAME_TEMPO_KEY = "LastGameTempo";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadStats();
            Debug.Log("GameStatsManager initialized");
        }
        else
        {
            Debug.LogWarning("Duplicate GameStatsManager destroyed");
            Destroy(gameObject);
        }
    }

    public void SaveStats()
    {
        PlayerPrefs.SetInt(TOTAL_WINS_KEY, totalWins);
        PlayerPrefs.SetFloat(TOTAL_TAP_TEMPO_KEY, totalTapTempo);
        PlayerPrefs.SetInt(GAMES_PLAYED_KEY, gamesPlayed);
        PlayerPrefs.SetFloat(LAST_GAME_TEMPO_KEY, lastGameTempo);
        PlayerPrefs.Save();
        Debug.Log("GameStats saved to PlayerPrefs");
    }

    public void LoadStats()
    {
        totalWins = PlayerPrefs.GetInt(TOTAL_WINS_KEY, 0);
        totalTapTempo = PlayerPrefs.GetFloat(TOTAL_TAP_TEMPO_KEY, 0f);
        gamesPlayed = PlayerPrefs.GetInt(GAMES_PLAYED_KEY, 0);
        lastGameTempo = PlayerPrefs.GetFloat(LAST_GAME_TEMPO_KEY, 0f);
        CalculateAverageTempo();
        Debug.Log("GameStats loaded from PlayerPrefs");
    }

    public void CalculateAverageTempo()
    {
        averageTapTempo = gamesPlayed > 0 ? totalTapTempo / gamesPlayed : 0;
    }

    public void RegisterGameResult(bool isWin, float gameTapTempo)
    {
        gamesPlayed++;
        if (isWin) totalWins++;
        totalTapTempo += gameTapTempo;
        lastGameTempo = gameTapTempo;
        CalculateAverageTempo();

        Debug.Log($"Registered game result: Win={isWin}, " +
                 $"Tempo={gameTapTempo:F2}, " +
                 $"TotalWins={totalWins}, " +
                 $"GamesPlayed={gamesPlayed}");
    }

    // ѕубличные свойства дл€ доступа к статистике
    public int TotalWins => totalWins;
    public float AverageTapTempo => averageTapTempo;
    public float LastGameTempo => lastGameTempo;

    // ѕриватные пол€ дл€ хранени€ данных
    private int totalWins;
    private float totalTapTempo;
    private int gamesPlayed;
    private float averageTapTempo;
    private float lastGameTempo;
}