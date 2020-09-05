using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UTime = UnityEngine.Time;
public class ScoreSystem : MonoBehaviour
{
    private static ScoreSystem Instance;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private GameObject textParent;

    [SerializeField]
    private TextMeshProUGUI accuracy;

    public static bool IsRunning { get; private set; }
    public static int Score { get; private set; } = 0;
    public static int HighScore { get; private set; } = 0;
    public static int Mulitplier { get; private set; } = 1;
    public static bool HasInitialized { get; private set; } = false;
    const int SCORE_INCREMENT_VALUE = 1;

    static float Time = 0;

    void OnEnable()
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
    /// Score System Cycle
    /// </summary>
    /// <returns></returns>
    public static IEnumerator SystemCycle()
    {
        while (true)
        {
            Time += UTime.deltaTime;

            if (Time >= ((60 / GameManager.CurrentSpeed) / 100) && IsRunning)
            {
                Score += Mulitplier * SCORE_INCREMENT_VALUE;
                UpdateHighScore();
                UpdateUI();
                Time = 0;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Update HighScore
    /// </summary>
    private static void UpdateHighScore()
    {
        HighScore = (Score > HighScore) ? Score : HighScore;
    }

    /// <summary>
    /// Update the Ui when it calls for ut
    /// </summary>
    static void UpdateUI()
    {
        //Formats number as 0000
        Instance.scoreText.text = Score.ToString();
    }

    /// <summary>
    /// Submit object value (score) to manager
    /// </summary>
    public static void SubmitToManager()
    {
        GameManager.ScoreSubmit(Score);

        //Only change the highscore if the score is greater than it.
        GameManager.HighScoreSubmit(HighScore);
        PlayFabLogin.SetCloudStats();
    }

    /// <summary>
    /// Stop system. Score will be sumbitted
    /// </summary>
    public static void Stop()
    {
        IsRunning = false;
        Instance.textParent.SetActive(IsRunning);
        SubmitToManager();
    }

    /// <summary>
    /// Continues System
    /// </summary>
    public static void Resume()
    {
        IsRunning = true;
        Instance.textParent.SetActive(IsRunning);
    }

    /// <summary>
    /// Initialize System
    /// </summary>
    public static void Init()
    {
        if (!HasInitialized)
        {
            IsRunning = true;
            Instance.textParent.SetActive(IsRunning);
            Instance.StartCoroutine(SystemCycle());
        }
    }

    /// <summary>
    /// Reset score to 0
    /// </summary>
    public static void ResetScore()
    {
        Score = 0;
        SubmitToManager();
    }

    /// <summary>
    /// Add value to score
    /// </summary>
    /// <param name="value"></param>
    public static void AddToScore(int value)
    {
        Score += value;
    }

    /// <summary>
    /// Update the highscore
    /// </summary>
    /// <param name="value"></param>
    internal static void UpdateHighScore(int value)
    {
        HighScore = value;
        SubmitToManager();
    }
}
