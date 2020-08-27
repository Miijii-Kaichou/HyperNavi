using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using Random = UnityEngine.Random;
public class GameOverScreen : MonoBehaviour, IUnityAdsListener
{
    public class EndGameComments
    {
        string[] comments;
        int atScore;

        /// <summary>
        /// Generate a variety of end game comments depending on score
        /// </summary>
        /// <param name="atScore"></param>
        /// <param name="comments"></param>
        public EndGameComments(int atScore, params string[] comments)
        {
            this.comments = comments;
            this.atScore = atScore;
        }

        public string[] GetComments() => comments;
        public int GetAtScore() => atScore;
    }

    [SerializeField]
    private TextMeshProUGUI comment;

    [SerializeField]
    private TextMeshProUGUI score;

    [SerializeField]
    private TextMeshProUGUI currencyAmount;

    private static IUnityAdsListener Instance;

    private static EventManager.Event @continueEvent, @startOverEvent, @gemsEvent;

    private static Result transactionResult;

    private static string currencyRemainFormat;

    private static EndGameComments[] endGameComments =
    {
        new EndGameComments(0,
            "You seem pretty tired...",
            "Should you be playing this at the state you're in?",
            "Take a break. You seem tired.",
            "You're out of it today aren't you?"),

        new EndGameComments(99,
            "I'm guessing you didn't take this too serious?",
            "First Time?",
            "You must be starting off.",
            "Getting used to the controls, huh?"),

        new EndGameComments(249,
            "Slowly Getting a hang of it",
            "Just keep practicing",
            "Perhaps better than when you first started?",
            "Good Score"),

        new EndGameComments(499,
            "Are you caughting on?",
            "Slightly better",
            "It may look easy, but it takes time.",
            "Never stop trying!"),

        new EndGameComments(749,
            "You're getting good at this...",
            "Very admirable!",
            "Did you see the difficulty increase?",
            "You're almost in the thousands!"),

        new EndGameComments(999,
            "Congrats! You've unlocked your hidden potential!",
            "You're officially not a beginner!",
            "Great Navigation!",
            "Go higher!!!"),

        new EndGameComments(2499,
            "You're at the highest peak!",
            "You're a professional at this game!",
            "You have amazing hand-eye coordination!",
            "Continue to test your limits!"),

    };

    private void Awake()
    {
        

        Instance = this;

        //Generate a random comment based on score
        GenerateComment();

        //Post Score to Screen
        PostScore();

        PostCurrency();

        continueEvent = EventManager.AddNewEvent(999, "continue", () =>
        {
            

            //Spawn player to last signal point
            ProceduralGenerator.DontDeactivate();
            GameManager.ResetTime();
            GameManager.SpawnPlayerToLastSignalPoint();
            Advertisement.RemoveListener(Instance);
            EventManager.RemoveEvent("continue");
        });

        startOverEvent = EventManager.AddNewEvent(998, "startOver", () =>
        {
            ProceduralGenerator.DontDeactivate();
            GameManager.ResetTime();
            //Spawn player to last signal point
            GameManager.SpawnPlayerToStartLayout();
            Advertisement.RemoveListener(Instance);
            EventManager.RemoveEvent("startOver");
        });

        gemsEvent = EventManager.AddNewEvent(997, "getGemsWithVideo", () =>
        {
            CurrencySystem.AddToBalance(50);
            PostCurrency();
        });

        Advertisement.AddListener(Instance);

#if UNITY_ANDROID
        Advertisement.Initialize("3789069");

#elif UNITY_IOS
        Advertisement.Initialize("3789068");

#endif
    }


    public void OnTryAgain()
    {
        GameManager.UnloadScene("GameOverScene", startOverEvent);
    }

    public void OnContinue()
    {
        if(SufficientCurrentcy())
            GameManager.UnloadScene("GameOverScene", continueEvent);
    }

    /// <summary>
    /// Player gets extra live, but they have to watch an ad
    /// </summary>
    public void GetGems()
    {
        ShowAd();
    }

    void ShowAd(string zone = "rewardedVideo")
    {
        while (!Advertisement.IsReady(zone)) continue;
        Advertisement.Show(zone);
    }


    public void OnUnityAdsReady(string placementId)
    {

    }

    public void OnUnityAdsDidError(string message)
    {

    }

    public void OnUnityAdsDidStart(string placementId)
    {

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
                //TODO: Add a message saying that an error 
                //had occurred with the ad.
                //Just don't do anything for now.
                return;

            case ShowResult.Skipped:
                //Remove Listener
                //TODO: You are allowed 10 secs to skip
                //Give the player a chance to look at it for 10 seconds
                //As opposed to the whole entire video.
                gemsEvent.Trigger();
                return;

            case ShowResult.Finished:
                //Finally, reward the player for there efforts
                gemsEvent.Trigger();
                return;
            default:
                break;
        }
    }

    /// <summary>
    /// Generate a comment
    /// </summary>
    private void GenerateComment()
    {
        int size = endGameComments.Length;

        for (int iter = 0; iter < size; iter++)
        {
            EndGameComments comment = endGameComments[iter];
            EndGameComments nextComment;
            try
            {
                nextComment = iter < size ? endGameComments[iter + 1] : comment;
            }
            catch
            {
                nextComment = comment;
            }

            int atScore = comment.GetAtScore();
            int nextAtScore = nextComment.GetAtScore();

            if (
                (iter < size &&
                comment != null &&
                GameManager.CurrentScore >= atScore &&
                GameManager.CurrentScore <= nextAtScore) ||
                (iter >= size && GameManager.CurrentScore >= atScore))
            {
                PickComment(comment);
                return;
            }
        }
    }

    /// <summary>
    /// Pick a random comment from end comments
    /// </summary>
    /// <param name="endComment"></param>
    private void PickComment(EndGameComments endComment)
    {
        string[] comments = endComment.GetComments();
        int size = comments.Length;
        int value = Random.Range(0, size - 1);
        comment.text = comments[value];
    }

    /// <summary>
    /// Post Score on GameOver Screen
    /// </summary>
    private void PostScore()
    {
        score.text = GameManager.CurrentScore.ToString();
    }

    /// <summary>
    /// Post Remaining Currency on GameOver Screen
    /// </summary>
    private void PostCurrency()
    {
        currencyRemainFormat = string.Format("Remaining: {0}", GameManager.CurrentCurrency);
        currencyAmount.text = currencyRemainFormat;
    }

    /// <summary>
    /// Check if there is sufficient funds
    /// </summary>
    /// <returns></returns>
    bool SufficientCurrentcy()
    {
        CurrencySystem.ExecuteTransaction(1000, out transactionResult);
        int resultValue = (int)transactionResult;
        return resultValue.ToBoolean();
    }
}
