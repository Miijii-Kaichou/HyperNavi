using System.Collections;
using UnityEngine;

public class PlayerController : Controller
{
    [SerializeField]
    TouchDetection movementTouchArea, boostTouchArea;

    PlayerPawn player;

    protected override void OnEnable()
    {
        StartCoroutine(ControllerCycle());
        Init();
    }

    /// <summary>
    /// Main Controller Cycle
    /// </summary>
    /// <returns></returns>
    IEnumerator ControllerCycle()
    {
        while (true)
        {
            if (GameManager.IsGameStarted)
            {
                InputControls();
                TouchInputControls();
            }

            yield return null;
        }
    }

    /// <summary>
    /// Initialize Player Controller
    /// </summary>
    protected override void Init()
    {
        Keymapper.Configure(
            new Keymapper.Key("left", KeyCode.LeftArrow),
            new Keymapper.Key("right", KeyCode.RightArrow),
            new Keymapper.Key("up", KeyCode.UpArrow),
            new Keymapper.Key("down", KeyCode.DownArrow),
            new Keymapper.Key("boost", KeyCode.Space));

         player = AssociatedPawn as PlayerPawn;
    }

    void InputControls()
    {
        
        //Movement controls
        if (Keymapper.OnKeyDown("left") &&
            player.GetDirection() != Direction.RIGHT &&
            player.CanTurn())
        {
            player.MoveLeft();
            player.ProhibitTurn();
        }

        else if (Keymapper.OnKeyDown("right") &&
            player.GetDirection() != Direction.LEFT &&
            player.CanTurn())
        {
            player.MoveRight();
            player.ProhibitTurn();
        }

        if (Keymapper.OnKeyDown("up") &&
            player.GetDirection() != Direction.DOWN &&
            player.CanTurn())
        {
            player.MoveUp();
            player.ProhibitTurn();
        }

        else if (Keymapper.OnKeyDown("down") &&
            player.GetDirection() != Direction.UP &&
            player.CanTurn())
        {
            player.MoveDown();
            player.ProhibitTurn();
        }

        //Boosting
        if (Keymapper.OnKeyDown("boost"))
        {
            player.Boost();
        }
    }

    void TouchInputControls()
    {
        //Movement controls
        if (movementTouchArea.SlideLeft() &&
            player.GetDirection() != Direction.RIGHT &&
            player.CanTurn())
        {
            player.MoveLeft();
            player.ProhibitTurn();
        }

        else if (movementTouchArea.SlideRight() &&
            player.GetDirection() != Direction.LEFT &&
            player.CanTurn())
        {
            player.MoveRight();
            player.ProhibitTurn();
        }

        else if (movementTouchArea.SlideUp() &&
            player.GetDirection() != Direction.DOWN &&
            player.CanTurn())
        {
            player.MoveUp();
            player.ProhibitTurn();
        }

        else if (movementTouchArea.SlideDown() &&
            player.GetDirection() != Direction.UP &&
            player.CanTurn())
        {
            player.MoveDown();
            player.ProhibitTurn();
        }

        //Boosting
        if (boostTouchArea.Tap())
        {
            player.Boost();
        }
    }

    /// <summary>
    /// Assign the movement detection to the player controller
    /// </summary>
    /// <param name="detection"></param>
    public void AssignMovementDetection(TouchDetection detection) => movementTouchArea = detection;

    /// <summary>
    /// Assign the boost detectionn to the player controller
    /// </summary>
    /// <param name="detection"></param>
    public void AssignBoostDetection(TouchDetection detection) => boostTouchArea = detection;
}
