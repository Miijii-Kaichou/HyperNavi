using UnityEngine;
using UnityEngine.Advertisements;
public class GameOverScreen : MonoBehaviour, IUnityAdsListener
{
    private static IUnityAdsListener Instance;

    private static EventManager.Event @continueEvent, @startOverEvent;

    private void Awake()
    {
        Instance = this;

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

    /// <summary>
    /// Player gets extra live, but they have to watch an ad
    /// </summary>
    public void OnContinue()
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
                GameManager.UnloadScene("GameOverScene", continueEvent);
                return;

            case ShowResult.Finished:
                //Finally, unload this scene
                GameManager.UnloadScene("GameOverScene", continueEvent);
                return;
            default:
                break;
        }
    }
}
