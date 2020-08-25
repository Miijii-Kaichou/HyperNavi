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

    /// <summary>
    /// Start of Object
    /// </summary>
    private void Start()
    {
        player = GameManager.player;
    }

    /// <summary>
    /// On Enable
    /// </summary>
    private void OnEnable()
    {
        generator = transform.parent.parent.GetComponent<ProceduralGenerator>();
        generateEvent = EventManager.AddNewEvent(42, "generateLayout", () => SignalGeneration());
    }

    /// <summary>
    /// Calculate distance between point a and b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    void CalculateDistance(Transform a, Transform b)
    {
        distance = Vector3.Distance(b.position, a.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null)
        {
            if (turningPoint) player.AllowTurn();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Signal generator to generate a layout based on the player's direction
        if (collision.CompareTag("Player"))
            generateEvent.Trigger();
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
}
