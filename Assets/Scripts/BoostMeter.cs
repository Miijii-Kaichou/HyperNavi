using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UTime = UnityEngine.Time;

/// <summary>
/// How much boost you can use 
/// </summary>
public class BoostMeter : MonoBehaviour
{
    private static BoostMeter Instance;

    [SerializeField]
    private Slider boostMeter;

    [SerializeField]
    private GameObject boostParent;

    public static float BoostAmount { get; private set; }

    //This is the value that the BoostAmount will lerp to
    public static float BoostAmountValue { get; private set; }

    //The delta for lerping
    public static float BoostMeterDelta { get; private set; } = 0.1f;

    //This is the rate in which BoostAmount lerps to the BoostAmountValue
    public static float BoostMeterRate { get; private set; } = 0.01f;

    private const float METER_MIN_VALUE = 0;
    private const float METER_MAX_VALUE = 100;

    //Time
    private static float Time = 0;

    //Is Running
    public static bool  IsRunning { get; private set; }

    //Coroutine
    static IEnumerator boostSystemCycle;

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
    }

    /// <summary>
    /// Lerping between current and the difference of boost value
    /// making a smooth decrease effect for the meter
    /// </summary>
    /// <returns></returns>
    static IEnumerator SystemCycle()
    {
        while (true)
        {
            Time += UTime.deltaTime;

            //If BoostAmount is not equal to BoostAmountValue, 
            //start lerping to that value
            if (Time >= BoostMeterRate && !BoostAmount.Equals(BoostAmountValue) && IsRunning)
            {
                Debug.Log(BoostAmountValue);
                BoostAmount = Mathf.Lerp(BoostAmount, BoostAmountValue, BoostMeterDelta);
                UpdateUI();
                Time = 0;
            }

            //Regulate boost meter value
            RegulateBoostMeter();

            yield return null;
        }

    }

    /// <summary>
    /// Make sure the boost amount value is not greater than or less than
    /// its max or min value
    /// </summary>
    static void RegulateBoostMeter()
    {
        if (BoostAmount <= METER_MIN_VALUE)
            BoostAmount = METER_MIN_VALUE;

        if (BoostAmount >= METER_MAX_VALUE)
            BoostAmount = METER_MAX_VALUE;
    }

    public static bool SufficientValue()
        => BoostAmountValue > 0;

    /// <summary>
    /// Update the Ui whenever you need to
    /// </summary>
    static void UpdateUI()
         => Instance.boostMeter.value = BoostAmount;

    public static void DepleteBoost(float value)
        => BoostAmountValue -= value;

    public static void ReplenishBoost(float value)
        => BoostAmountValue += value;

    public static void Stop()
    {
        IsRunning = false;

        //Disable Parent
        Instance.boostParent.SetActive(IsRunning);
    }

    public static void Resume()
    {
        IsRunning = true;

        //Disable Parent
        Instance.boostParent.SetActive(IsRunning);
    }

    public static void ResetSystem()
    {
        Init(false);
    }

    /// <summary>
    /// Initialize object
    /// </summary>
    public static void Init(bool startCoroutine = true)
    {
        IsRunning = true;

        //Assign IEnumerator
        boostSystemCycle = SystemCycle();

        //Enable Parent
        Instance.boostParent.SetActive(IsRunning);

        //Set min and max value of sliders
        Instance.boostMeter.minValue = METER_MIN_VALUE;
        Instance.boostMeter.maxValue = METER_MAX_VALUE;

        //BoostAmount is equal to METER_MAX_VALUE
        //and BoostAmountValue is equal to BoostAmount
        BoostAmount = METER_MAX_VALUE;
        BoostAmountValue = BoostAmount;

        //Update the UI
        UpdateUI();


        if (startCoroutine)
        {
            //Start Boost Meter Cycle
            Instance.StartCoroutine(boostSystemCycle);
        }
    }
}
