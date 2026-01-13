using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button.CompareTag("Button") || button.CompareTag("UnlockedLevelButton") )
        {
            button.onClick.AddListener(PlaySound);
        }

    }

    void PlaySound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
        else
        {
            Debug.LogError("AudioManager instance not found!");
        }
    }

    void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(PlaySound);
    }
}