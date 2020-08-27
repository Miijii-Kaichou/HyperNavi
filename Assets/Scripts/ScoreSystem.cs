using System.Collections;
using TMPro;
using UnityEngine;

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

    public static IEnumerator SystemCycle()
    {
        while (true)
        {
            if (IsRunning)
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
        Instance.scoreText.text = Score.ToString();
    }

    public static void SubmitToManager()
    {
        GameManager.ScoreSubmit(Score);
    }

    /// <summary>
    /// Stop system. Score will be sumbitted
    /// </summary>
    public static void Stop() {
        IsRunning = false;
        Instance.textParent.SetActive(IsRunning); 
    }

    /// <summary>
    /// Continues System
    /// </summary>
    public static void Resume()
    {
        IsRunning = true;
    }

    /// <summary>
    /// Initialize System
    /// </summary>
    public static void Init()
    {
        
        IsRunning = true;
        Instance.textParent.SetActive(IsRunning);
        Instance.StartCoroutine(SystemCycle());
    }

    public static void ResetScore()
    {
        Score = 0;
        SubmitToManager();
    }

    public static void AddToScore(int value)
    {
        Score += value;
        SubmitToManager();
    }
}
