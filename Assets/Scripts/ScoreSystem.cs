using System.Collections;
using System.Globalization;
using System.Security.AccessControl;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private static ScoreSystem Instance;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI accuracy;

    public static bool IsRunnning { get; private set; }
    public static int Score { get; private set; } = 0;
    public static int Mulitplier { get; private set; } = 1;
    const int SCORE_INCREMENT_VALUE = 1;

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

        GameManager.SetScoreSystem(Instance);
    }

    static IEnumerator ScoreSystemCycle()
    {
        while (true)
        {
            if (IsRunnning)
            {
                Score += Mulitplier * SCORE_INCREMENT_VALUE;
                UpdateUI();

                yield return new WaitForSeconds((60 / GameManager.CurrentSpeed) / 100);
            }

            yield return null;
        }
    }

    static void UpdateUI()
    {
        //Formats number as 0000
        Instance.scoreText.text = Score.ToString("D4", CultureInfo.InvariantCulture);
    }

    public static void SubmitToManager(int score)
    {
        GameManager.ScoreSubmit(score);
    }

    /// <summary>
    /// Stop system. Score will be sumbitted
    /// </summary>
    public void Stop() {
        IsRunnning = false;
        SubmitToManager(Score);
    }

    public void Resume()
    {
        IsRunnning = true;
    }

    /// <summary>
    /// Initialize System
    /// </summary>
    public void Init()
    {
        scoreText.transform.parent.gameObject.SetActive(true);
        IsRunnning = true;
        StartCoroutine(ScoreSystemCycle());
    }

    public void ResetScore()
    {
        Score = 0;
    }

    public void AddToScore(int value)
    {
        Score += value;
    }
}
