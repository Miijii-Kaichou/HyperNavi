using UnityEngine;
using UnityEngine.Advertisements;
public class GameOverScreen : MonoBehaviour, IUnityAdsListener
{
    private static IUnityAdsListener Instance;

    private void Awake()
    {
        Instance = this;
        
        Advertisement.AddListener(Instance);
        Advertisement.Initialize("3789069");
    }

    public void OnTryAgain()
    {
        GameManager.UnloadScene("GameOverScene", EventManager.AddNewEvent(998, "continue", () =>
        {
            //Spawn player to last signal point
            GameManager.SpawnPlayerToStartLayout();
            Advertisement.RemoveListener(Instance);
        }));
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
                GameManager.UnloadScene("GameOverScene", EventManager.AddNewEvent(999, "continue", () =>
                {
                    //Spawn player to last signal point
                    GameManager.SpawnPlayerToLastSignalPoint();
                    Advertisement.RemoveListener(Instance);
                }));
                return;

            case ShowResult.Finished:
                //Finally, unload this scene
                GameManager.UnloadScene("GameOverScene", EventManager.AddNewEvent(999, "continue", () =>
                {
                    //Spawn player to last signal point
                    GameManager.SpawnPlayerToLastSignalPoint();
                    Advertisement.RemoveListener(Instance);;
                }));
                return;
            default:
                break;
        }
    }
}
