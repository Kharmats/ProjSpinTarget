using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMainMenu : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnQuickExit);
        }
    }

    public void OnQuickExit()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InstantQuitToMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
