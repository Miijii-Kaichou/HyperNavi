using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public void Unload(string sceneName)
    {
        GameManager.UnloadScene(sceneName);
    }

    /// <summary>
    /// When the "Play" Option's been selected
    /// </summary>
    public void OnPlay()
    {
        GameManager.StartGame();
        ProceduralGenerator.OnPreview = false;
        animator.SetBool("GameStarted", GameManager.IsGameStarted);
    }

    public void OnOptions()
    {
        GameManager.LoadScene("Options");
        GameManager.UnloadScene(GameManager.TitleScreenName());
    }

    public void OnCredits()
    {

    }

    public void OnRecords()
    {

    }

    public void OnFacebookLink()
    {

    }

    public void OnInstagramLink()
    {

    }

    public void OnTwitterLink()
    {

    }

    public void OnLinkedInLink()
    {

    }
}
