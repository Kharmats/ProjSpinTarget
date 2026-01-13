using UnityEngine;

public class ShopInitializer : MonoBehaviour
{
    void Start()
    {
        // Создаем GameShopManager если не существует
        if (GameShopManager.Instance == null)
        {
            GameObject obj = new GameObject("GameShopManager");
            obj.AddComponent<GameShopManager>();
        }

        // Инициализируем CurrencyManager если не существует
        if (CurrencyManager.Instance == null)
        {
            GameObject currencyObj = new GameObject("CurrencyManager");
            currencyObj.AddComponent<CurrencyManager>();
            currencyObj.GetComponent<CurrencyManager>().LoadCoins();
        }

        // Принудительно обновляем все элементы
        GameShopManager.Instance.RefreshAllItems();
    }
}