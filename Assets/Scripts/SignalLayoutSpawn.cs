using UnityEngine;

public class SignalLayoutSpawn : MonoBehaviour, IRange
{
    ProceduralGenerator generator;

    /// <summary>
    /// Layout that this trigger belongs to
    /// </summary>
    [SerializeField]
    OpeningPath layout;

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

    void Start()
    {
        Init();
    }

    public OpeningPath GetPath() => layout;

    /// <summary>
    /// Trigger an event to generate a new layout
    /// </summary>
    void SignalGeneration()
    {
        if (!ProceduralGenerator.Exists()) return;

        if (!generator.dontDeactivate && ProceduralGenerator.PreviousLayout != null)
            ProceduralGenerator.PreviousLayout.gameObject.SetActive(false);

        generator.dontDeactivate = false;



        if (player != null)
        {
            Side side;
            OpeningPath path = null;
            switch (player.GetDirection())
            {
                case Direction.LEFT:
                    //Coming from left. Need right to open
                    side = Side.RIGHT;
                    generator.GenerateLayout(side, layout, ref path);
                    return;

                case Direction.RIGHT:
                    //Coming from Right. Need left to open
                    side = Side.LEFT;
                    generator.GenerateLayout(side, layout, ref path);
                    return;

                case Direction.UP:
                    //Coming from top. Need bottome to open
                    side = Side.BOTTOM;
                    generator.GenerateLayout(side, layout, ref path);
                    return;

                case Direction.DOWN:
                    //Coming from bottom. Need top to open
                    side = Side.TOP;
                    generator.GenerateLayout(side, layout, ref path);
                    return;

                default:
                    return;
            }
        }
    }

    public void SubmitDistanceToManager()
    {
        GameManager.DetermineTiming(distanceCheck.GetDistance());
    }

    private void Init()
    {
        generator = transform.parent.parent.GetComponent<ProceduralGenerator>();
    }


    /// <summary>
    /// Start of Object
    /// </summary>
    private void OnEnable()
    {
        player = GameManager.player;
    }

    public void OnRangeEnter()
    {
        if (player != null && turningPoint)
            player.AllowTurn();

        if (ProceduralGenerator.Exists() && !ProceduralGenerator.IsStalling)
        {
            SignalLayoutSpawn signal = layout.GetSignal();
            ProceduralGenerator.PreviousLayout = ProceduralGenerator.CurrentLayout;
            ProceduralGenerator.CurrentLayout = layout;
            ProceduralGenerator.FlushPaths();
            player.UpdateSignalPoint(signal);
        }
    }

    public void OnRangeExit()
    {
        //Signal generator to generate a layout based on the player's direction
        SignalGeneration();
        player.ProhibitTurn();
        GameManager.AllowDestructionOfPlayer();
        GameManager.ResetTime();
    }
}
