using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StretchSpriteToScreen : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Camera targetCamera;
    private int lastWidth;
    private int lastHeight;
    private Vector3 initialScale;

    void Awake()
    {
        // Явно устанавливаем начальный масштаб 100x100
        initialScale = new Vector3(100f, 100f, 1f);
        transform.localScale = initialScale;
    }

    void Start()
    {
        InitializeComponents();
        StretchSprite();
    }

    void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component missing!", this);
            enabled = false;
            return;
        }

        if (targetCamera == null) targetCamera = Camera.main;

        if (targetCamera == null)
        {
            Debug.LogError("No camera found!", this);
            enabled = false;
        }
    }

    void StretchSprite()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null || targetCamera == null)
        {
            Debug.LogWarning("Cannot stretch sprite: components missing");
            return;
        }

        // Получаем размер спрайта с учетом начального масштаба
        Vector2 spriteSize = GetScaledSpriteSize();

        // Вычисляем размер экрана
        Vector2 screenSize = GetScreenWorldSize();

        // Рассчитываем новый масштаб
        Vector3 newScale = initialScale;
        newScale.x *= screenSize.x / spriteSize.x;
        newScale.y *= screenSize.y / spriteSize.y;

        transform.localScale = newScale;

        Debug.Log($"Stretched sprite: initial={initialScale}, new={newScale}, " +
                  $"spriteSize={spriteSize}, screenSize={screenSize}");
    }

    Vector2 GetScaledSpriteSize()
    {
        Vector2 size = spriteRenderer.sprite.bounds.size;
        size.x *= initialScale.x;
        size.y *= initialScale.y;
        return size;
    }

    Vector2 GetScreenWorldSize()
    {
        if (targetCamera.orthographic)
        {
            float height = targetCamera.orthographicSize * 2f;
            return new Vector2(height * targetCamera.aspect, height);
        }
        else
        {
            float distance = Mathf.Abs(transform.position.z - targetCamera.transform.position.z);
            float height = 2.0f * distance * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            return new Vector2(height * targetCamera.aspect, height);
        }
    }

    void Update()
    {
        if (targetCamera == null) return;

        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            StretchSprite();
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }
}