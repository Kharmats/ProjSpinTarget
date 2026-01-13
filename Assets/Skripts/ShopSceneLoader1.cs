using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopSceneLoader1 : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // При переходе из магазина сбрасываем GameShopManager
        if (sceneName != "ShopScene" && GameShopManager.Instance != null)
        {
            Destroy(GameShopManager.Instance.gameObject);
        }
        SceneManager.LoadScene(sceneName);
    }
}