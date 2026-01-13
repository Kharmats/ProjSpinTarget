using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [System.Serializable]
    public class LevelInfo
    {
        public int levelNumber;
        public bool isUnlocked;
        public string unlockedButtonPath;
        public string lockedButtonPath;

        [System.NonSerialized] public Button unlockedButton;
        [System.NonSerialized] public Button lockedButton;
    }

    public List<LevelInfo> levels = new List<LevelInfo>();
    public int currentLevel = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LevelsScene")
        {
            FindLevelButtons();
            UpdateLevelButtons();
            SetupButtonListeners();
        }
    }

    private void FindLevelButtons()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (LevelInfo level in levels)
        {
            level.unlockedButton = null;
            level.lockedButton = null;

            if (!string.IsNullOrEmpty(level.unlockedButtonPath))
            {
                foreach (GameObject obj in allObjects)
                {
                    if (GetFullPath(obj.transform) == level.unlockedButtonPath)
                    {
                        level.unlockedButton = obj.GetComponent<Button>();
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(level.lockedButtonPath))
            {
                foreach (GameObject obj in allObjects)
                {
                    if (GetFullPath(obj.transform) == level.lockedButtonPath)
                    {
                        level.lockedButton = obj.GetComponent<Button>();
                        break;
                    }
                }
            }
        }
    }

    private string GetFullPath(Transform transform)
    {
        if (transform == null) return "";

        string path = transform.name;
        Transform current = transform;
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }
        return path;
    }

    public void CompleteCurrentLevel()
    {
        UnlockLevel(currentLevel + 1);
        PlayerPrefs.SetInt("LastLevel", currentLevel);
        PlayerPrefs.Save();
    }

    public void UnlockLevel(int levelNumber)
    {
        LevelInfo level = levels.Find(l => l.levelNumber == levelNumber);
        if (level != null)
        {
            if (!level.isUnlocked)
            {
                level.isUnlocked = true;
                PlayerPrefs.SetInt($"Level_{levelNumber}_Unlocked", 1);
                SaveProgress();

                if (SceneManager.GetActiveScene().name == "LevelsScene")
                {
                    UpdateLevelButtons();
                }
            }
        }
        else
        {
            Debug.LogError($"Level {levelNumber} not found in levels list");
        }
    }

    private void UpdateLevelButtons()
    {
        foreach (LevelInfo level in levels)
        {
            if (level.unlockedButton != null)
            {
                level.unlockedButton.gameObject.SetActive(level.isUnlocked);
            }

            if (level.lockedButton != null)
            {
                level.lockedButton.gameObject.SetActive(!level.isUnlocked);
            }
        }
    }

    private void SetupButtonListeners()
    {
        foreach (LevelInfo level in levels)
        {
            if (level.unlockedButton != null)
            {
                level.unlockedButton.onClick.RemoveAllListeners();
                level.unlockedButton.onClick.AddListener(() => LoadLevel(level.levelNumber));
            }
        }
    }

    private void SaveProgress()
    {
        foreach (LevelInfo level in levels)
        {
            PlayerPrefs.SetInt($"Level_{level.levelNumber}_Unlocked", level.isUnlocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        foreach (LevelInfo level in levels)
        {
            int unlockedValue = PlayerPrefs.GetInt($"Level_{level.levelNumber}_Unlocked", 0);
            level.isUnlocked = unlockedValue == 1;

            if (level.levelNumber == 1 && !level.isUnlocked)
            {
                level.isUnlocked = true;
                PlayerPrefs.SetInt($"Level_1_Unlocked", 1);
            }
        }

        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
    }

    public void LoadLevel(int level)
    {
        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.SetInt("LastLevel", level);
        PlayerPrefs.Save();
        SceneManager.LoadScene($"GameScene{level}");
    }

#if UNITY_EDITOR
    [ContextMenu("Auto Fill Paths")]
    void AutoFillPaths()
    {
        List<bool> activeStates = new List<bool>();
        List<GameObject> objectsToRestore = new List<GameObject>();

        foreach (LevelInfo level in levels)
        {
            if (level.unlockedButton != null)
            {
                activeStates.Add(level.unlockedButton.gameObject.activeSelf);
                objectsToRestore.Add(level.unlockedButton.gameObject);
                level.unlockedButton.gameObject.SetActive(true);
            }

            if (level.lockedButton != null)
            {
                activeStates.Add(level.lockedButton.gameObject.activeSelf);
                objectsToRestore.Add(level.lockedButton.gameObject);
                level.lockedButton.gameObject.SetActive(true);
            }
        }

        foreach (LevelInfo level in levels)
        {
            if (level.unlockedButton != null)
            {
                level.unlockedButtonPath = GetFullPath(level.unlockedButton.transform);
            }

            if (level.lockedButton != null)
            {
                level.lockedButtonPath = GetFullPath(level.lockedButton.transform);
            }
        }

        for (int i = 0; i < objectsToRestore.Count; i++)
        {
            objectsToRestore[i].SetActive(activeStates[i]);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
#endif
}