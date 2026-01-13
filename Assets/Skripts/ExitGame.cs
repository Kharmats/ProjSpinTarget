using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void QuitGame()
    {
        // Для выхода в редакторе во время тестирования
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Для финальной сборки на Android
        Application.Quit();
#endif
    }
}