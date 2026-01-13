using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public int SelectedBallSkin => PlayerPrefs.GetInt("SelectedBallSkin", 0);
    public int SelectedTargetSkin => PlayerPrefs.GetInt("SelectedTargetSkin", 0);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaultSkins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDefaultSkins()
    {
        // Гарантируем, что скины по умолчанию куплены и выбраны
        if (!PlayerPrefs.HasKey("Item_BallSkin_0_Purchased"))
            PlayerPrefs.SetInt("Item_BallSkin_0_Purchased", 1);

        if (!PlayerPrefs.HasKey("Item_TargetSkin_0_Purchased"))
            PlayerPrefs.SetInt("Item_TargetSkin_0_Purchased", 1);

        if (!PlayerPrefs.HasKey("SelectedBallSkin"))
            PlayerPrefs.SetInt("SelectedBallSkin", 0);

        if (!PlayerPrefs.HasKey("SelectedTargetSkin"))
            PlayerPrefs.SetInt("SelectedTargetSkin", 0);

        PlayerPrefs.Save();
    }

    public void ApplyAllSkinsImmediately()
    {
       
        Debug.Log("Applying all skins...");
    }
}