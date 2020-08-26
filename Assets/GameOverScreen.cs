using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class GameOverScreen : MonoBehaviour, IUnityAdsListener
{
    private static IUnityAdsListener Instance;

    private void Awake()
    {
        Instance = this;
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

    void ShowAd(string zone = "rewardedVideoZone")
    {
        Advertisement.AddListener(Instance);

        StartCoroutine(GetReady(zone));
    }

    IEnumerator GetReady(string placementId)
    {
        while (!Advertisement.IsReady(placementId))
        {
            yield return new WaitUntil(() => Advertisement.IsReady(placementId));

            OnUnityAdsReady(placementId);
        }
    }


    public void OnUnityAdsReady(string placementId)
    {
        if (!Advertisement.isShowing)
            Advertisement.Show(placementId);
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
                break;
            case ShowResult.Skipped:
                Debug.Log("Ad was skipped.");
                //Remove Listener
                break;
            case ShowResult.Finished:
                Debug.Log("Yay! Ad is now finished!!!");

                Advertisement.RemoveListener(Instance);

                //Finally, unload this scene
                GameManager.UnloadScene("GameOverScene", EventManager.AddNewEvent(999, "continue", () =>
                {
                    //Spawn player to last signal point
                    GameManager.SpawnPlayerToLastSignalPoint();
                }));
                break;
            default:
                break;
        }
    }
}
