using UnityEngine;

public class BallSkinApplier : MonoBehaviour
{
    [System.Serializable]
    public struct BallSkin
    {
        public int skinId;
        public Material material;
        public Sprite sprite; // Добавлена поддержка спрайтов
    }

    [Header("Skin Settings")]
    public BallSkin[] availableSkins;
    public Renderer ballRenderer;
    public SpriteRenderer ballSpriteRenderer; // Для спрайтов

    void Start()
    {
        ApplySelectedSkin();
    }

    public void ApplySelectedSkin()
    {
        int selectedSkinId = PlayerPrefs.GetInt("SelectedBallSkin", 0);

        foreach (var skin in availableSkins)
        {
            if (skin.skinId == selectedSkinId)
            {
                // Применяем материал или спрайт
                if (skin.material != null && ballRenderer != null)
                {
                    ballRenderer.material = skin.material;
                }

                if (skin.sprite != null && ballSpriteRenderer != null)
                {
                    ballSpriteRenderer.sprite = skin.sprite;
                }
                break;
            }
        }
    }
}