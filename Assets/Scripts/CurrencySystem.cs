using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance;
    public static int currencyBalance { get; private set; }

    public static bool IsRunnning { get; private set; }

    [SerializeField]
    private TextMeshProUGUI currencyAmount;

    [SerializeField]
    private UnityEvent onSpend = new UnityEvent();

    void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    static IEnumerator SystemCycle()
    {
        while (true)
        {
            if (IsRunnning)
                UpdateUI();

            yield return new WaitForSeconds(1 / 60);
        }
    }

    static void UpdateUI()
    {
        //Formats number as 0000
        Instance.currencyAmount.text = currencyBalance.ToString();
    }

    public static void AddToBalance(int value) => currencyBalance += value;

    static void WithdrawFromBalance(int value) => currencyBalance -= value;

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

    public static void Init()
    {
        IsRunnning = true;
        Instance.StartCoroutine(SystemCycle());
    }

    /// <summary>
    /// Stop system. Score will be sumbitted
    /// </summary>
    public static void Stop()
    {
        IsRunnning = false;
    }

    /// <summary>
    /// Continues System
    /// </summary>
    public static void Resume()
    {
        IsRunnning = true;
    }

    static bool IsSufficientAmount(int requestedAmount) => currencyBalance >= requestedAmount;
}
