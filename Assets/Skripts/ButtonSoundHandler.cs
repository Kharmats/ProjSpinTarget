using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSoundHandler : MonoBehaviour
{
    void Awake()
    {
        // јвтоматически создаем инстанс при старте игры
        if (AudioManager.Instance == null)
        {
            GameObject audioManager = new GameObject("AudioManager");
            audioManager.AddComponent<AudioManager>();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SubscribeAllButtons();
    }

    void SubscribeAllButtons()
    {
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button btn in buttons)
        {
            if (btn.CompareTag("Button") && btn.gameObject.scene.isLoaded)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => AudioManager.Instance?.PlayButtonSound());
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}