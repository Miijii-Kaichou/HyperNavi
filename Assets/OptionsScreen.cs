using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsScreen : MonoBehaviour
{
    /// <summary>
    /// This scene should always be on unless returning
    /// to the main title screen
    /// </summary>
    [SerializeField]
    string optionSceneName;

    string currentScene = null;
    string previousScene = null;

    // Start is called before the first frame update
    void Start()
    {
        currentScene = null;
        previousScene = null;
        OnGeneral();
    }

    public void BackToTitleMenu()
    {
        GameManager.UnloadScene(currentScene);
        currentScene = null;
        previousScene = null;
        GameManager.UnloadScene("Options");
        GameManager.LoadScene("TitleScreen", true, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Event for clicking/tapping on General Tab
    /// </summary>
    public void OnGeneral()
    {
        previousScene = currentScene;

        if (previousScene != "")
            GameManager.UnloadScene(previousScene);

        currentScene = "General";
        GameManager.LoadScene(currentScene, true, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Event for clicking/tapping on OnAudio Tab
    /// </summary>
    public void OnAudio()
    {
        
        previousScene = currentScene;

        if (previousScene != "")
            GameManager.UnloadScene(previousScene);

        currentScene = "Audio";
        GameManager.LoadScene(currentScene, true, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Event for clicking/tapping on Graphic Tab
    /// </summary>
    public void OnGraphics()
    {

        previousScene = currentScene;

        if (previousScene != "")
            GameManager.UnloadScene(previousScene);

        previousScene = currentScene;
        currentScene = "Graphics";
        GameManager.LoadScene(currentScene, true, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Event for clicking/tapping on Gem Shop Icon
    /// </summary>
    public void OnGemShop()
    {

        previousScene = currentScene;

        if (previousScene != "")
            GameManager.UnloadScene(previousScene);

        currentScene = "GemShop";
        GameManager.LoadScene(currentScene, true, LoadSceneMode.Additive);

    }
}
