using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance;

    /// <summary>
    /// Current User Balance 
    /// </summary>
    public static int CurrencyBalance { get; private set; }

    /// <summary>
    /// Check if the system is running
    /// </summary>
    public static bool IsRunning { get; private set; }

    /// <summary>
    /// Check if it also has been initialized
    /// </summary>
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
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }

        #endregion
        systemCycle = SystemCycle();
    }

    /// <summary>
    /// System Cycle
    /// </summary>
    /// <returns></returns>
    static IEnumerator SystemCycle()
    {
        while (true)
        {
            //If running, update the UI
            if (IsRunning)
                UpdateUI();

            //Do this every 1/60 of a second
            yield return new WaitForSeconds(1 / 60);
        }
    }

    /// <summary>
    /// Update Currency UI
    /// </summary>
    static void UpdateUI()
    {
        //Formats number as 0000
        Instance.currencyAmount.text = CurrencyBalance.ToString();
    }

    /// <summary>
    /// Add to current balance
    /// </summary>
    /// <param name="value"></param>
    public static void AddToBalance(int value)
    {
        CurrencyBalance += value;
        SubmitToManager();
    }

    /// <summary>
    /// Take from balance
    /// </summary>
    /// <param name="value"></param>
    static void WithdrawFromBalance(int value)
    {
        CurrencyBalance -= value;
        SubmitToManager();
    }

    /// <summary>
    /// Execute transactions
    /// </summary>
    /// <param name="valueAmount"></param>
    /// <param name="result"></param>
    public static void ExecuteTransaction(int valueAmount, out Result result)
    {
        //Check to see if the player has
        //sufficient funds
        if (IsSufficientAmount(valueAmount))
        {
            //Withdraw the specified amount
            WithdrawFromBalance(valueAmount);

            //Trigger the "onSpend" event
            Instance.onSpend.Invoke();

            //Let us know that the Transaction was successful
            result = Result.SUCCESS;
            return;
        }

#if UNITY_EDITOR
        Debug.Log("Insufficient Funds");
#endif

        //Failed to do any transaction
        result = Result.FAILURE;
    }

    /// <summary>
    /// Initialize Currency System
    /// </summary>
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
