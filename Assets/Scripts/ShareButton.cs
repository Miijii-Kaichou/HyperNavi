using System.Collections;
using System.IO;
using UnityEngine;
using TMPro;

public class ShareButton : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputMessageField;

    private string shareMessage;
    private string message = null;

    NativeShare share = null;

    private bool done = false;

    //Coroutines
    IEnumerator writingCycle, screenShotCycle;

    private void Start()
    {
        writingCycle = WritingMessage();
        screenShotCycle = TakeScreenShotAndShare();
        StartCoroutine(writingCycle);
    }

    void OnShare()
    {
        //Create our share message
        /*What I can do with this is allow the person to make their own message as
         they share. We'll have a string Format that takes in the users message,
        and then add the amount of points and such with the share message*/
        shareMessage =
            message + "\n" + GameManager.CurrentScore.ToString() + " points " +
            "in HyperNavi!!!";

        StartCoroutine(screenShotCycle);
    }

    /// <summary>
    /// Take a screenshot of the game before sharing
    /// </summary>
    /// <returns></returns>
    private IEnumerator TakeScreenShotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "sharedImage.png");
        File.WriteAllBytes(filePath, screenShot.EncodeToPNG());

        //We destroy the testru to avoid any memory leaks
        Destroy(screenShot);

        share = new NativeShare();
        share.AddFile(filePath).SetSubject("HyperNavi").SetText(shareMessage).Share();

        shareMessage.ClearString();
        message.ClearString();
        inputMessageField.text.ClearString();
    }

    /// <summary>
    /// The process of writing a message.
    /// </summary>
    /// <returns></returns>
    IEnumerator WritingMessage()
    {
        while (true)
        {
            message =
            inputMessageField.text;
            if (done)
            {
                OnShare();
                done = false;
                StopCoroutine(writingCycle);
            }

            yield return null;
        }
    }

    public void OnSend() => done = true;
}
