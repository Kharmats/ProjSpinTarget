using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float orbitRadius = 3f;
    [Range(0.1f, 10f)]
    public float baseOrbitSpeed = 1f;
    public Transform orbitCenter;

    [Header("Game Reference")]
    public GameManager gameManager;

    private float currentAngle;
    private bool isMoving = true;
    private float orbitSpeed;

    void Start()
    {
        InitializeBall();
        ApplySkin();

        // Применяем текущую скорость
        UpdateRotationSpeed(SettingsManager.CurrentRotationSpeed);

        // Подписываемся на будущие изменения
        SettingsManager.OnRotationSpeedChanged += UpdateRotationSpeed;
    }

    void OnDestroy()
    {
        SettingsManager.OnRotationSpeedChanged -= UpdateRotationSpeed;
    }

    private void UpdateRotationSpeed(float newSpeedPercent)
    {
        orbitSpeed = baseOrbitSpeed * (newSpeedPercent / 100f);
    }

    private void InitializeBall()
    {
        if (orbitCenter == null) orbitCenter = GameObject.Find("OrbitCenter")?.transform;
        if (gameManager == null) gameManager = FindObjectOfType<GameManager>();

        if (orbitCenter == null || gameManager == null)
        {
            Debug.LogError("BallController: Critical references missing!");
            enabled = false;
            return;
        }

        currentAngle = Random.Range(0, Mathf.PI * 2);
        UpdatePosition();
    }

    private void ApplySkin()
    {
        if (GameSettings.Instance == null) return;

        int selectedSkinId = GameSettings.Instance.SelectedBallSkin;
        string skinPath = $"BallSkins/skin_{selectedSkinId}";
        Sprite skinSprite = Resources.Load<Sprite>(skinPath);

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null && skinSprite != null)
        {
            renderer.sprite = skinSprite;
        }
        else
        {
            Debug.LogWarning($"Ball skin not found: {skinPath}");
        }
    }

    private void HandleSpeedChanged(float newSpeedPercent)
    {
        orbitSpeed = baseOrbitSpeed * (newSpeedPercent / 100f);
    }

    void Update()
    {
        // Не двигаемся, если показывается статистика
        if (gameManager != null && gameManager.statsPanel.activeSelf)
        {
            return;
        }

        if (!isMoving || orbitCenter == null) return;

        currentAngle += orbitSpeed * Time.deltaTime;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float x = Mathf.Cos(currentAngle) * orbitRadius;
        float y = Mathf.Sin(currentAngle) * orbitRadius;
        transform.position = orbitCenter.position + new Vector3(x, y, 0);
    }

    public void HandlePlayerInput()
    {
        if (!isMoving || gameManager == null) return;

        // Получаем позицию ввода
        Vector2 inputPosition = Vector2.zero;

#if UNITY_EDITOR
        inputPosition = Input.mousePosition;
#else
        if (Input.touchCount > 0) inputPosition = Input.GetTouch(0).position;
#endif

        // Проверяем, находится ли ввод в активной зоне
        if (gameManager != null && !gameManager.IsPointInActiveZone(inputPosition))
        {
            Debug.Log("Input in top dead zone, ignored.");
            return;
        }

        // Регистрируем нажатие
        gameManager.RegisterPlayerTap();

        if (MultiTargetController.Instance != null &&
            MultiTargetController.Instance.activeTarget != null)
        {
            float distanceToTarget = Vector2.Distance(
                transform.position,
                MultiTargetController.Instance.activeTarget.transform.position
            );

            if (distanceToTarget < gameManager.targetRadius)
            {
                gameManager.RegisterHit();
            }
            else
            {
                gameManager.RegisterMiss();
            }
        }
        else
        {
            gameManager.RegisterMiss();
        }
    }

    public void StopMovement()
    {
        isMoving = false;
    }
}