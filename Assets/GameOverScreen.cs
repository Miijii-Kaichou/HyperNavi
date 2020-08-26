using System.Collections;
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
        Advertisement.Show(zone);
    }


    public void OnUnityAdsReady(string placementId)
    {

    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log(message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Ad has started");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
                OnUnityAdsDidError("Ad came across an error.");
                return;
            case ShowResult.Skipped:
                Debug.Log("Ad was skipped.");
                //Remove Listener
                return;
            case ShowResult.Finished:
                Debug.Log("Yay! Ad is now finished!!!");;

                //Finally, unload this scene
                GameManager.UnloadScene("GameOverScene", EventManager.AddNewEvent(999, "continue", () =>
                {
                    //Spawn player to last signal point
                    GameManager.SpawnPlayerToLastSignalPoint();
                    Advertisement.RemoveListener(Instance);
                }));
                return;
            default:
                break;
        }
    }
}
