using UnityEngine;

public class CharacterEmotionController : MonoBehaviour
{
    [Header("Emotion Settings")]
    public GameObject normalEmotionObject; // Объект с нормальной эмоцией
    public GameObject sadEmotionObject;    // Объект с грустной эмоцией

    void Start()
    {
        SetNormalEmotion();
    }

    public void SetNormalEmotion()
    {
        if (normalEmotionObject != null)
        {
            normalEmotionObject.SetActive(true);
        }
        if (sadEmotionObject != null)
        {
            sadEmotionObject.SetActive(false);
        }
    }

    public void SetSadEmotion()
    {
        if (normalEmotionObject != null)
        {
            normalEmotionObject.SetActive(false);
        }
        if (sadEmotionObject != null)
        {
            sadEmotionObject.SetActive(true);
        }
    }
}