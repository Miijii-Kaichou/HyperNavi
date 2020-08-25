using UnityEngine;

public class SignalLayoutSpawn : MonoBehaviour
{
    ProceduralGenerator generator;

    /// <summary>
    /// Layout that this trigger belongs to
    /// </summary>
    [SerializeField]
    OpeningPath layout;

    /// <summary>
    /// States if there's a turning point
    /// </summary>
    [SerializeField]
    bool turningPoint = true;


    float distance = 0f;

    /// <summary>
    /// The Player
    /// </summary>
    PlayerPawn player;

    /// <summary>
    /// Generation Event
    /// </summary>
    EventManager.Event @generateEvent;

    DistanceCheck distanceCheck;

    /// <summary>
    /// Start of Object
    /// </summary>
    private void Start()
    {
        player = GameManager.player;

        //Get distance check component
        distanceCheck = GetComponent<DistanceCheck>();

        generator = transform.parent.parent.GetComponent<ProceduralGenerator>();
        generateEvent = EventManager.AddNewEvent(42, "generateLayout", () => SignalGeneration());

        //Set distance target
        distanceCheck.SetTarget(player.transform);

        //Set up OnRangeEnter Event
        distanceCheck.OnRangeEnter.AddNewListener(
        () =>
        {
            if (player != null && turningPoint)
            {
                player.AllowTurn();
                player.UpdateSignalPoint(layout.GetSignal());
            }
        });


        //Set up OnRangeExit Event
        distanceCheck.OnRangeExit.AddNewListener(
        () =>
        {
            //Signal generator to generate a layout based on the player's direction
            generateEvent.Trigger();
            player.ProhibitTurn();
            GameManager.AllowDestructionOfPlayer();
        });
    }

    private void Update()
    {
        if (player != null)
            distance = distanceCheck.Distance;
    }

    /// <summary>
    /// Trigger an event to generate a new layout
    /// </summary>
    void SignalGeneration()
    {
        generator.previousLayout = generator.currentLayout;

        if (!generator.dontDeactivate && generator.previousLayout != null)
            generator.previousLayout.gameObject.SetActive(false);

        generator.currentLayout = layout;

        generator.dontDeactivate = false;

        if (player != null)
        {
            Side side;

            switch (player.GetDirection())
            {
                case Direction.LEFT:
                    //Coming from left. Need right to open
                    side = Side.RIGHT;
                    generator.GenerateLayout(side, layout);
                    return;

                case Direction.RIGHT:
                    //Coming from Right. Need left to open
                    side = Side.LEFT;
                    generator.GenerateLayout(side, layout);
                    return;

                case Direction.UP:
                    //Coming from top. Need bottome to open
                    side = Side.BOTTOM;
                    generator.GenerateLayout(side, layout);
                    return;

                case Direction.DOWN:
                    //Coming from bottom. Need top to open
                    side = Side.TOP;
                    generator.GenerateLayout(side, layout);
                    return;

                default:
                    return;
            }
        }
    }

    public void SubmitDistanceToManager()
    {
        GameManager.DetermineTiming(distance);
    }
}
