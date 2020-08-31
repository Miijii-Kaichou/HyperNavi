using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance;
    public static int CurrencyBalance { get; private set; }

    public static bool IsRunning { get; private set; }

    public static bool HasInitialized { get; private set; } = false;

    [SerializeField]
    private TextMeshProUGUI currencyAmount;

    [SerializeField]
    private GameObject textParent;

    [SerializeField]
    private UnityEvent onSpend = new UnityEvent();

    //Coroutines
    IEnumerator systemCycle;

    private void Awake()
    {
        systemCycle = SystemCycle();
    }

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
            if (IsRunning)
                UpdateUI();

            yield return new WaitForSeconds(1 / 60);
        }
    }

    static void UpdateUI()
    {
        //Formats number as 0000
        Instance.currencyAmount.text = CurrencyBalance.ToString();
    }

    public static void AddToBalance(int value)
    {
        CurrencyBalance += value;
        SubmitToManager();
    }

    static void WithdrawFromBalance(int value)
    {
        CurrencyBalance -= value;
        SubmitToManager();
    }

    public static void ExecuteTransaction(int valueAmount, out Result result)
    {
        if (IsSufficientAmount(valueAmount))
        {
            WithdrawFromBalance(valueAmount);
            Instance.onSpend.Invoke();
            result = Result.SUCCESS;
            return;
        }

#if UNITY_EDITOR
        Debug.Log("Insufficient Funds");
#endif

        result = Result.FAILURE;
    }

    public static void Init()
    {
        if (!HasInitialized)
        {
            IsRunning = true;
            Instance.textParent.SetActive(IsRunning);
            Instance.StartCoroutine(Instance.systemCycle);
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log("Currency System already initialized");
        }
#endif
    }

    /// <summary>
    /// Stop system. Score will be sumbitted
    /// </summary>
    public static void Stop()
    {
        IsRunning = false;
        Instance.textParent.SetActive(IsRunning);
        Instance.StopCoroutine(Instance.systemCycle);
        SubmitToManager();
    }

    /// <summary>
    /// Continues System
    /// </summary>
    public static void Resume()
    {
        IsRunning = true;
        Instance.textParent.SetActive(IsRunning);
        Instance.StartCoroutine(Instance.systemCycle);
    }


    /// <summary>
    /// Submit value to Game Manager
    /// </summary>
    /// <param name="score"></param>
    public static void SubmitToManager()
    {
        GameManager.CurrencySumbit(CurrencyBalance);
    }

    static bool IsSufficientAmount(int requestedAmount) => CurrencyBalance >= requestedAmount;
}
