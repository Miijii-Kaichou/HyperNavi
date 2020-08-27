using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance;
    public static int currencyBalance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI currencyAmount;

    [SerializeField]
    private UnityEvent onSpend = new UnityEvent();

    void Start()
    {
        GameManager.SetCurrencySystem(Instance);
    }

    public static void AddToBalance(int value) => currencyBalance += value;

    public static void WithdrawFromBalance(int value) => currencyBalance -= value;

    public static void ExecuteTransaction(int valueAmount, out Result result)
    {
        if (IsSufficientAmount(valueAmount))
        {
            WithdrawFromBalance(valueAmount);
            Instance.onSpend.Invoke();
            result = Result.SUCCESS;
            return;
        }

        result = Result.FAILURE;
    }

    static bool IsSufficientAmount(int requestedAmount) => currencyBalance >= requestedAmount;
}
