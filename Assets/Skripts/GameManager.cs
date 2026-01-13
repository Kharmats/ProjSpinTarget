using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameStatsManager statsManager;

    private Coroutine showStatsCoroutine;
    private bool isQuitting = false; // Флаг для отслеживания выхода

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioSource audioSourceLose;
    public AudioSource audioSourceWin;
    public AudioClip clip;


    [Header("Target Settings")]
    public MultiTargetController targetController;

    [Header("Game Settings")]
    public int maxAttempts = 5;
    public float targetRadius = 1.5f;
    public int coinsToWin = 30;

    [Header("References")]
    public BallController ball;
    public Text attemptsText;
    public Text coinsText;
    public GameObject hitPlank;
    public GameObject missPlank;
    public GameObject statsPanel;
    public Text accuracyText;
    public CharacterEmotionController characterEmotionController;
    public float missDelay = 1.5f;
    private List<float> clickTimes = new List<float>(); // Для отслеживания времени кликов
    private List<float> hitAccuracies = new List<float>();
    private float averageAccuracy;
    private int currentAttempts;
    private int coinsEarned;
    private bool gameActive = true;
    private bool gameEnded = false;
    private int currentLevel;

    [Header("Input Settings")]
    public float topDeadZoneHeight = 100f; // Высота неактивной зоны в пикселях

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Гарантируем создание GameStatsManager
        EnsureStatsManagerExists();
    }

    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        if (targetController != null)
        {
            targetController.InitializeTargets(currentLevel);
        }

        currentAttempts = maxAttempts;
        coinsEarned = 0;
        gameEnded = false;
        UpdateUI();
        accuracyText.text = "0%";

        if (characterEmotionController != null)
        {
            characterEmotionController.SetNormalEmotion();
        }
        clickTimes.Clear();
    }
    public void PlayEffect()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
    public void PlayEffectWin()
    {
        if (audioSource != null)
        {
            audioSourceWin.Play();
        }
    }
    public void PlayEffectLose()
    {
        if (audioSource != null)
        {
            audioSourceLose.Play();
        }
    }

    private void EnsureStatsManagerExists()
    {
        statsManager = FindObjectOfType<GameStatsManager>();

        if (statsManager == null)
        {
            GameObject statsObj = new GameObject("GameStatsManager");
            statsManager = statsObj.AddComponent<GameStatsManager>();
            DontDestroyOnLoad(statsObj);
            Debug.Log("Created new GameStatsManager instance");
        }
    }

    public bool IsPointInActiveZone(Vector2 screenPoint)
    {
        // Проверяем, находится ли точка в верхней "мертвой зоне"
        return screenPoint.y < Screen.height - topDeadZoneHeight;
    }
    public void RegisterPlayerTap()
    {
        clickTimes.Add(Time.time);
        Debug.Log($"Tap registered at {Time.time}. Total taps: {clickTimes.Count}");
    }
    public void RegisterHit()
    {
        if (!gameActive || gameEnded) return;

        if (ball == null) return;

        if (targetController != null && targetController.activeTarget != null)
        {
            float distanceToTarget = Vector2.Distance(
                ball.transform.position,
                targetController.activeTarget.transform.position
            );

            if (distanceToTarget < targetRadius / 2.1)
            {
                PlayEffect();
                coinsEarned += 10;
                float accuracy = Mathf.Clamp(100 - (distanceToTarget / targetRadius * 100), 0, 100);
                hitAccuracies.Add(accuracy);
                UpdateAccuracyUI();
                UpdateUI();

                targetController.OnTargetHit(targetController.activeTarget);

                if (coinsEarned >= coinsToWin)
                {
                    PlayEffectWin();
                    EndGame(true);
                    return;
                }
            }
            else
            {
                
                RegisterMiss();
                return;
            }
        }
        else
        {
            
            RegisterMiss();
            return;
        }

        
        currentAttempts--;
        UpdateUI();
        ShowPlank(hitPlank);
        UpdateUI();
        CheckGameEnd();
    }

    private void UpdateAccuracyUI()
    {
        if (hitAccuracies.Count > 0)
        {
            float sum = 0f;
            foreach (float acc in hitAccuracies) sum += acc;
            averageAccuracy = sum / hitAccuracies.Count;
        }
        else
        {
            averageAccuracy = 0f;
        }

        accuracyText.text = $" {averageAccuracy:F1}%";
    }

    public void RegisterMiss()
    {
        Debug.Log($"RegisterMiss called. gameActive: {gameActive}, gameEnded: {gameEnded}, isQuitting: {isQuitting}");

        PlayEffectLose();
        if (!gameActive || gameEnded || isQuitting)
        {
            Debug.Log("RegisterMiss aborted: game not active or ending");
            return;
        }

        currentAttempts--;
        Debug.Log($"Attempt decreased. Current attempts: {currentAttempts}");
        ShowPlank(missPlank);
        UpdateUI();

        if (ball != null)
        {
            ball.StopMovement();
        }

        if (characterEmotionController != null)
        {
            characterEmotionController.SetSadEmotion();
        }

        // Проверяем окончание игры
        if (currentAttempts <= 0)
        {
            Debug.Log("Attempts exhausted! Processing loss...");
            ProcessLoss();
        }
        else
        {
            Debug.Log("Showing stats panel without ending game");
            statsPanel.SetActive(true);
        }
    }

    private void ProcessLoss()
    {
        Debug.Log("ProcessLoss started");

        if (gameEnded)
        {
            Debug.LogWarning("ProcessLoss aborted: game already ended");
            return;
        }

        gameEnded = true;
        statsPanel.SetActive(true);

        // Рассчитываем темп кликов
        float gameTapTempo = CalculateGameTapTempo();
        Debug.Log($"Game lost. Tempo: {gameTapTempo:F2}");

        // Сохраняем статистику
        if (statsManager != null)
        {
            statsManager.RegisterGameResult(false, gameTapTempo);
            statsManager.SaveStats();
            Debug.Log("Loss stats saved successfully");
        }
        else
        {
            Debug.LogError("GameStatsManager is missing during loss!");
        }

        // Начисляем монеты
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(coinsEarned);
            Debug.Log($"Added coins on loss: {coinsEarned}");
        }
        else
        {
            Debug.LogError("CurrencyManager instance not available during loss!");
        }

        // Дополнительная отладочная информация
        Debug.Log($"Coins earned: {coinsEarned}, Attempts left: {currentAttempts}");
    }

    private IEnumerator ShowStatsAfterDelay()
    {
        yield return new WaitForSeconds(missDelay);

        if (isQuitting) yield break; // Прерываем если идёт выход

        statsPanel.SetActive(true);

        bool isWin = (coinsEarned >= coinsToWin);
        float gameTapTempo = CalculateGameTapTempo();

        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.RegisterGameResult(isWin, gameTapTempo);
        }
    }

    public void InstantQuitToMainMenu()
    {
        // Устанавливаем флаг выхода
        isQuitting = true;

        // Останавливаем все корутины
        if (showStatsCoroutine != null)
        {
            StopCoroutine(showStatsCoroutine);
        }

        // Сбрасываем состояние игры
        gameActive = false;
        gameEnded = true;

        // Останавливаем движение мяча
        if (ball != null)
        {
            ball.StopMovement();
        }

        // Скрываем все панели
        hitPlank.SetActive(false);
        missPlank.SetActive(false);
        statsPanel.SetActive(false);

        // Загружаем главное меню без задержки
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowPlank(GameObject plank)
    {

        plank.SetActive(true);
        Invoke("HidePlanks", 1f);
        UpdateUI();
    }

    private void HidePlanks()
    {
        hitPlank.SetActive(false);
        missPlank.SetActive(false);
    }

    private void UpdateUI()
    {
        attemptsText.text = $" {currentAttempts}";
        coinsText.text = $" {coinsEarned}";
    }

    private void CheckGameEnd()
    {
        if (currentAttempts <= 0 && !gameEnded)
        {
            if (coinsEarned >= coinsToWin)
            {
                EndGame(true);
            }
            else
            {
                ProcessLoss(); // Записываем статистику поражения
            }
        }
    }

    private void EndGame(bool isWin)
    {
        if (gameEnded) return;

        gameEnded = true;
        currentAttempts = 0;
        UpdateUI();
        statsPanel.SetActive(true);

        float gameTapTempo = CalculateGameTapTempo();
        Debug.Log($"Game ended. Win: {isWin}, Tempo: {gameTapTempo:F2}");

        if (statsManager != null)
        {
            statsManager.RegisterGameResult(isWin, gameTapTempo);
            statsManager.SaveStats();
            Debug.Log("Stats saved successfully");
        }

        // Начисляем монеты только при победе
        if (isWin && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(coinsEarned);
            Debug.Log($"Added coins on win: {coinsEarned}");
        }

        // Действия при победе
        if (isWin)
        {
            PlayEffectWin();
            int nextLevel = currentLevel + 1;
            PlayerPrefs.SetInt($"Level_{nextLevel}_Unlocked", 1);
            PlayerPrefs.SetInt("LastLevel", nextLevel);

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.UnlockLevel(nextLevel);
            }
        }

        PlayerPrefs.Save();
    }

    private float CalculateGameTapTempo()
    {
        if (clickTimes.Count < 2)
        {
            Debug.LogWarning("Not enough taps to calculate tempo");
            return 0f;
        }

        float totalTime = 0f;
        for (int i = 1; i < clickTimes.Count; i++)
        {
            totalTime += clickTimes[i] - clickTimes[i - 1];
        }

        float averageTime = totalTime / (clickTimes.Count - 1);
        return 60f / averageTime; // кликов в минуту
    }

    public void RestartGame()
    {
        if (characterEmotionController != null)
        {
            characterEmotionController.SetNormalEmotion();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}