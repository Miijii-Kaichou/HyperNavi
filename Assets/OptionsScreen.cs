using System;
using UnityEditorInternal;
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

    // Start is called before the first frame update
    void Start()
    {
        GameManager.SceneIndex = GameManager.GetSceneIndexByName("General");
        Load();
    }

    public void BackToTitleMenu()
    {

    }

    /// <summary>
    /// Event for clicking/tapping on General Tab
    /// </summary>
    public void OnGeneral()
    {
        GameManager.SceneIndex = GameManager.GetSceneIndexByName("General");
        Load();
    }

    /// <summary>
    /// Event for clicking/tapping on OnAudio Tab
    /// </summary>
    public void OnAudio()
    {
        GameManager.SceneIndex = GameManager.GetSceneIndexByName("Audio");
        Load();
    }

    /// <summary>
    /// Event for clicking/tapping on Graphic Tab
    /// </summary>
    public void OnGraphics()
    {
        GameManager.SceneIndex = GameManager.GetSceneIndexByName("Graphics");
        Load();
    }

    /// <summary>
    /// Event for clicking/tapping on Gem Shop Icon
    /// </summary>
    public void OnGemShop()
    {
        GameManager.SceneIndex = GameManager.GetSceneIndexByName("GemShop");
        Load();
    }


    /// <summary>
    /// Load in a scene
    /// </summary>
    public static void Load()
    {
        string currentSceneName = GameManager.SceneNames()[GameManager.SceneIndex];

        GameManager.PreviousScene = GameManager.CurrentScene;

        if (GameManager.PreviousScene != null)
            GameManager.UnloadScene(GameManager.PreviousScene);

        GameManager.CurrentScene = currentSceneName;

        GameManager.LoadScene(currentSceneName, true, LoadSceneMode.Additive);
    }
}
