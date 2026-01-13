using UnityEngine;

public class GameVersionManager : MonoBehaviour
{
    // Укажите текущую версию вашей игры
    private int currentVersion = 11; // Обновляйте это число при релизах

    private const string VersionKey = "GameVersion";

    void Start()
    {
        // Получаем сохраненную версию из PlayerPrefs
        int savedVersion = PlayerPrefs.GetInt(VersionKey, 0);

        // Проверяем, если версии не совпадают
        if (savedVersion != currentVersion)
        {
            Debug.Log("Версия изменилась или первый запуск. Очистка PlayerPrefs...");
            PlayerPrefs.DeleteAll(); // очищаем все сохранения
            PlayerPrefs.SetInt(VersionKey, currentVersion); // сохраняем текущую версию
            PlayerPrefs.Save(); // обязательно вызываем Save()
        }
        else
        {
            Debug.Log("Версия совпадает. Продолжаем без очистки.");
        }
    }
}