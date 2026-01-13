using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class CustomSliderController : MonoBehaviour
{
    [Header("Slider Settings")]
    public Transform sliderHandle;
    public Transform startPoint;
    public Transform endPoint;
    public float currentValue = 50f;

    [Header("Value Display")]
    public TMP_Text valueText;
    public string valueSuffix = "%";


    // Делегат для обработки изменений
    public Action<float> OnValueChanged;

    public bool IsDragging { get; private set; }
    private Vector3 dragOffset;
    private Camera mainCamera;
    private int? currentTouchId = null;
    private Vector3 originalScale;

   

    void Start()
    {
        mainCamera = Camera.main;

        // Сохраняем исходный масштаб
        if (sliderHandle != null)
        {
            originalScale = sliderHandle.localScale;
        }

        // Автоматическое создание PhysicsRaycaster
        if (mainCamera != null && mainCamera.GetComponent<Physics2DRaycaster>() == null)
        {
            var raycaster = mainCamera.gameObject.AddComponent<Physics2DRaycaster>();
            raycaster.eventMask = LayerMask.GetMask("UI");
        }

        // Автоматическое создание EventSystem
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        UpdateSliderPosition();
        UpdateValueText();
    }

    void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        // Обработка касаний для мобильных устройств
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (!currentTouchId.HasValue && IsTouchOnHandle(touch.position))
                    {
                        currentTouchId = touch.fingerId;
                        StartDrag(touch.position);
                    }
                    break;

                case TouchPhase.Moved:
                    if (currentTouchId == touch.fingerId)
                    {
                        UpdateDragPosition(touch.position);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (currentTouchId == touch.fingerId)
                    {
                        EndDrag();
                    }
                    break;
            }
        }

        // Обработка мыши для редактора
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (IsTouchOnHandle(Input.mousePosition))
            {
                StartDrag(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButton(0) && IsDragging)
        {
            UpdateDragPosition(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
#endif
    }

    private bool IsTouchOnHandle(Vector2 screenPosition)
    {
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        // Проверка коллайдера
        Collider2D collider = sliderHandle.GetComponent<Collider2D>();
        if (collider != null)
        {
            return collider.OverlapPoint(worldPosition);
        }

        return false;
    }

    private void StartDrag(Vector2 screenPosition)
    {
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        Vector3 handlePosition = sliderHandle.position;
        Vector3 touchWorld = new Vector3(worldPosition.x, worldPosition.y, handlePosition.z);

        dragOffset = handlePosition - touchWorld;
        IsDragging = true;

        // Плавное увеличение масштаба
        sliderHandle.localScale = originalScale * 1.2f;
    }

    private void UpdateDragPosition(Vector2 screenPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = sliderHandle.position.z;

        Vector3 worldPoint = worldPosition + dragOffset;
        Vector3 closestPoint = GetClosestPointOnLine(worldPoint);
        sliderHandle.position = closestPoint;

        UpdateCurrentValue();
        UpdateValueText();
    }

    private void EndDrag()
    {
        IsDragging = false;
        currentTouchId = null;

        // Мгновенное восстановление масштаба
        sliderHandle.localScale = originalScale;
    }

    private void UpdateCurrentValue()
    {
        Vector3 line = endPoint.position - startPoint.position;
        Vector3 toHandle = sliderHandle.position - startPoint.position;

        float t = Vector3.Dot(toHandle, line.normalized) / line.magnitude;
        float newValue = Mathf.Clamp(t * 100f, 0f, 100f);

        if (Mathf.Abs(newValue - currentValue) > 0.1f)
        {
            currentValue = newValue;
            UpdateValueText();
            OnValueChanged?.Invoke(currentValue); // Вызываем делегат
        }
    }

    private void UpdateSliderPosition()
    {
        float t = currentValue / 100f;
        sliderHandle.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
    }

    public void UpdateValueText()
    {
        if (valueText != null)
        {
            valueText.text = Mathf.RoundToInt(currentValue) + valueSuffix;
        }
    }

    private Vector3 GetClosestPointOnLine(Vector3 point)
    {
        Vector3 line = endPoint.position - startPoint.position;
        Vector3 toPoint = point - startPoint.position;

        float t = Vector3.Dot(toPoint, line.normalized) / line.magnitude;
        t = Mathf.Clamp01(t);

        return startPoint.position + line * t;
    }

    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0f, 100f);
        UpdateSliderPosition();
        UpdateValueText();
        OnValueChanged?.Invoke(currentValue); // Уведомляем при программном изменении
    }
}