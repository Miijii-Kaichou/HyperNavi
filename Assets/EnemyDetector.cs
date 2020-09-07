using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDetector : MonoBehaviour
{
    public static EnemyDetector Instance;

    [SerializeField]
    Image warningSign;

    Color warningSignColor;

    static bool IsAlerting = false;
    bool on = false;

    //Constant Values
    const float ALPHA_VISIBLE = 1f;
    const float ALPHA_INVISIBLE = 0f;

    //Times
    float time = 0;
    float blinkTime = 0;
    const float BLICKING_RATE = 0.25f;
    const float DURATION = 2;

    //Coroutine
    IEnumerator blinkingCycle;

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

    private void OnEnable()
    {
        blinkingCycle = BlinkingCycle();
        StartCoroutine(blinkingCycle);
    }

    public static void SignalEnemyApproach()
    => IsAlerting = true;

    IEnumerator BlinkingCycle()
    {
        while (true)
        {
            if (IsAlerting && GameManager.IsGameStarted)
            {
                time += Time.deltaTime;
                blinkTime += Time.deltaTime;
                if (blinkTime >= BLICKING_RATE)
                {
                    ToggleVisibility();
                    blinkTime = 0;
                }

                if (time > DURATION)
                {
                    IsAlerting = false;
                    InvisibleSign();
                    time = 0;
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// Toggle Visibility on Signs
    /// </summary>
    void ToggleVisibility()
    {
        on = !on;
        warningSignColor = warningSign.color;
        warningSignColor = new Color(warningSignColor.r, warningSignColor.g, warningSignColor.b, (on == true) ? ALPHA_VISIBLE : ALPHA_INVISIBLE);
        warningSign.color = warningSignColor;
    }

    /// <summary>
    /// Set the sign invisible
    /// </summary>
    void InvisibleSign()
    {
        on = false;
        warningSignColor = warningSign.color;
        warningSignColor = new Color(warningSignColor.r, warningSignColor.g, warningSignColor.b, (on == true) ? ALPHA_VISIBLE : ALPHA_INVISIBLE);
        warningSign.color = warningSignColor;
    }

    public static bool Exists() => Instance != null;
}
