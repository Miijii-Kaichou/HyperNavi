using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// How much boost you can use 
/// </summary>
public class BoostMeter : MonoBehaviour
{
    [SerializeField]
    private Slider boostMeter;

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
    private float time = 0;

    //Coroutine
    IEnumerator boostMeterCycle;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    /// <summary>
    /// Lerping between current and the difference of boost value
    /// making a smooth decrease effect for the meter
    /// </summary>
    /// <returns></returns>
    IEnumerator BoostMeterCycle()
    {
        while (true)
        {
            time += Time.deltaTime;

            //If BoostAmount is not equal to BoostAmountValue, 
            //start lerping to that value
            if (time >= BoostMeterRate && !BoostAmount.Equals(BoostAmountValue))
            {
                BoostAmount = Mathf.Lerp(BoostAmount, BoostAmountValue, BoostMeterDelta);
                UpdateUI();
                time = 0;
            }

            yield return null;
        }

    }

    /// <summary>
    /// Update the Ui whenever you need to
    /// </summary>
   void UpdateUI()
   {
        boostMeter.value = BoostAmount;
   }

    public void DepleteBoost(float value)
    {
        BoostAmountValue -= value;
        Debug.Log(BoostAmountValue);
    }

    /// <summary>
    /// Initialize object
    /// </summary>
    void Init()
    {
        //Assign IEnumerator
        boostMeterCycle = BoostMeterCycle();

        //Set min and max value of sliders
        boostMeter.minValue = METER_MIN_VALUE;
        boostMeter.maxValue = METER_MAX_VALUE;

        //BoostAmount is equal to METER_MAX_VALUE
        //and BoostAmountValue is equal to BoostAmount
        BoostAmount = METER_MAX_VALUE;
        BoostAmountValue = BoostAmount;

        //Update the UI
        UpdateUI();

        //Start Boost Meter Cycle
        StartCoroutine(boostMeterCycle);
    }
}
