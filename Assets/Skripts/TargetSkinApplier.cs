using UnityEngine;

public class TargetSkinApplier : MonoBehaviour
{
    [System.Serializable]
    public struct TargetSkin
    {
        public int skinId;
        public Sprite sprite;
        public Color activeColor;
        public Color inactiveColor;
    }

    [Header("Skin Settings")]
    public TargetSkin[] availableSkins;
    public SpriteRenderer targetRenderer;

    private Color appliedActiveColor;
    private Color appliedInactiveColor;

    void Awake()
    {
        if (targetRenderer == null) targetRenderer = GetComponent<SpriteRenderer>();
        ApplySelectedSkin();
    }

    public void ApplySelectedSkin()
    {
        if (targetRenderer == null) targetRenderer = GetComponent<SpriteRenderer>();

        int selectedSkinId = PlayerPrefs.GetInt("SelectedTargetSkin", 0);

        foreach (var skin in availableSkins)
        {
            if (skin.skinId == selectedSkinId)
            {
                if (skin.sprite != null) targetRenderer.sprite = skin.sprite;
                break;
            }
        }

        // Просто применяем неактивный цвет как начальный
        // Активный цвет будет установлен позже через SetActiveState()
        targetRenderer.color = GetInactiveColor();
    }

    public Color GetActiveColor()
    {
        int selectedSkinId = PlayerPrefs.GetInt("SelectedTargetSkin", 0);
        foreach (var skin in availableSkins)
        {
            if (skin.skinId == selectedSkinId) return skin.activeColor;
        }
        return Color.white;
    }

    public Color GetInactiveColor()
    {
        int selectedSkinId = PlayerPrefs.GetInt("SelectedTargetSkin", 0);
        foreach (var skin in availableSkins)
        {
            if (skin.skinId == selectedSkinId) return skin.inactiveColor;
        }
        return new Color(1, 1, 1, 0.5f);
    }


}