using UnityEngine;

public class SignalLayoutSpawn : MonoBehaviour, IRange
{
    /// <summary>
    /// Layout that this trigger belongs to
    /// </summary>
    [SerializeField]
    OpeningPath currentPath;
    OpeningPath nextPath = null;

    [SerializeField]
    DistanceCheck distanceCheck;

    /// <summary>
    /// States if there's a turning point
    /// </summary>
    [SerializeField]
    bool turningPoint = true;

    /// <summary>
    /// The Player
    /// </summary>
    PlayerPawn player;

    public OpeningPath GetPath() => currentPath;

    EventManager.Event submit;

    void Start()
    {
        submit = EventManager.AddNewEvent(EventManager.FreeValue, "submit", () => 
        {
            GameManager.DetermineTiming(distanceCheck.GetDistance());
        });
    }

    /// <summary>
    /// Trigger an event to generate a new layout
    /// </summary>
    void SignalGeneration()
    {
        if (!ProceduralGenerator.IsGenerating)
        {
            if (!ProceduralGenerator.Exists()) return;

            if (!ProceduralGenerator.DontDeactivate && ProceduralGenerator.PreviousPath != null)
                ProceduralGenerator.PreviousPath.gameObject.SetActive(false);

            ProceduralGenerator.DontDeactivate = false;

            if (player != null)
            {
                Side side = default;

                EventManager.CallBackMethod generate = ()=>
                    ProceduralGenerator.GenerateLayout(side, currentPath, nextPath);

                ProceduralGenerator.GenerateEvent.AddNewListener(generate);
                switch (player.GetDirection())
                {
                    case Direction.LEFT:
                        //Coming from left. Need right to open
                        side = Side.RIGHT;
                        ProceduralGenerator.GenerateEvent.Trigger();
                        break;

                    case Direction.RIGHT:
                        //Coming from Right. Need left to open
                        side = Side.LEFT;
                        ProceduralGenerator.GenerateEvent.Trigger();
                        break;

                    case Direction.UP:
                        //Coming from top. Need bottome to open
                        side = Side.BOTTOM;
                        ProceduralGenerator.GenerateEvent.Trigger();
                        break;

                    case Direction.DOWN:
                        //Coming from bottom. Need top to open
                        side = Side.TOP;
                        ProceduralGenerator.GenerateEvent.Trigger();
                        break;
                }
                ProceduralGenerator.GenerateEvent.RemoveListener(generate);
                return;
            }
        }
    }

    public void SubmitDistanceToManager()
    {
        submit.Trigger();
        return;
    }

    /// <summary>
    /// Start of Object
    /// </summary>
    private void OnEnable()
    {
        
        player = GameManager.player;
    }

    /// <summary>
    /// Enter into range of signal
    /// </summary>
    public void OnRangeEnter()
    {
        player.UpdateSignalPoint(this);

        if (player != null && turningPoint)
            player.AllowTurn();

        if (ProceduralGenerator.Exists() && !ProceduralGenerator.IsStalling)
        {
            ProceduralGenerator.PreviousPath = ProceduralGenerator.CurrentPath;
            ProceduralGenerator.CurrentPath = currentPath;
            ProceduralGenerator.StripPaths();
        }
    }

    /// <summary>
    /// Exit out of the signal range
    /// </summary>
    public void OnRangeExit()
    {
        player.ProhibitTurn();
        GameManager.AllowDestructionOfPlayer();
        GameManager.ResetTime();

        //Signal generator to generate a layout based on the player's direction
        SignalGeneration();
    }
}
