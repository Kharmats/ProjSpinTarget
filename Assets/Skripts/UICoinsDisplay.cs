using TMPro;
using UnityEngine;

public class UICoinsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;

    void Start()
    {
        if (CurrencyManager.Instance != null && coinsText != null)
        {
            CurrencyManager.Instance.RegisterCoinsText(coinsText);
        }
    }

    void OnDestroy()
    {
        if (CurrencyManager.Instance != null && coinsText != null)
        {
            CurrencyManager.Instance.UnregisterCoinsText(coinsText);
        }
    }
}