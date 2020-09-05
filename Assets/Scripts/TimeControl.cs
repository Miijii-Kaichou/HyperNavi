using System.Collections;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    private static TimeControl Instance;
    private static float CurrentTimeScale
    {
        get
        {
            return Time.timeScale;
        }
        set
        {
            Time.timeScale = value;
        }
    }

    public static float PreviousTimeScale { get; private set; }

    private float scaleValue = 1f;
    private readonly float scaleDelta = 0.25f;
    private readonly float scaleRate = 0.01f;
    private const float FREEZE = 0;

    public static bool IsPaused { get; private set; } = false;
    //Coroutines
    IEnumerator timeLerpCycle;

    void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        timeLerpCycle = TimeLerpCycle();
        StartCoroutine(timeLerpCycle);
    }

    /// <summary>
    /// Set the slowdown time
    /// </summary>
    /// <param name="value"></param>
    public static void SlowDownTime(float value = 0.5f)
    {
        Instance.scaleValue = value;
    }

    /// <summary>
    /// This enumerator will be responsible for lerping back into one
    /// whenever he hit an enemy
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeLerpCycle()
    {
        while (true)
        {
            CurrentTimeScale = scaleValue;
            if (!IsPaused && CurrentTimeScale < 1)
            {
                //Lerp back to one
                scaleValue = Mathf.Lerp(scaleValue, 1f, scaleDelta);
            }

            //Check if the applciation is focued
            if (!Application.isFocused)
                Pause();

            yield return new WaitForSecondsRealtime(scaleRate);
        }
    }

    /// <summary>
    /// Pause the game.
    /// </summary>
    public static void Pause()
    {
        Instance.scaleValue = FREEZE;
    }

    /// <summary>
    /// When the application pauses
    /// </summary>
    /// <param name="paused"></param>
    public static void OnApplicationPause(bool paused)
    {
        PreviousTimeScale = CurrentTimeScale;
        CurrentTimeScale = FREEZE;
        IsPaused = paused;
    }

    /// <summary>
    /// When application is on focus
    /// </summary>
    static void OnApplicationFocus()
    {
        CurrentTimeScale = PreviousTimeScale;
    }
}
