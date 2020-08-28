using System.Collections;
using UnityEngine;

public class SignalLayoutSpawn : MonoBehaviour
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


    float distance = 0f;

    /// <summary>
    /// The Player
    /// </summary>
    PlayerPawn player;

    /// <summary>
    /// Generation Event
    /// </summary>
    EventManager.Event @generateEvent;

    void Start()
    {
        
        Init();
    }

    IEnumerator DistanceCheckingLoop()
    {
        while (true)
        {
            if (player != null)
                distance = distanceCheck.Distance;
#if UNITY_EDITOR
            else
                Debug.Log("Player is null");
#endif

            yield return null;
        }
    }

    public OpeningPath GetPath() => layout;

    /// <summary>
    /// Trigger an event to generate a new layout
    /// </summary>
    void SignalGeneration()
    {
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

    public void ClampPathAmount(int value)
    {
        OpeningPath[] paths = ObjectPooler.pooledObjects.GetAllPaths();
        int pathAmount = 0;
        for (int iter = 0; iter < paths.Length; iter++)
        {
            OpeningPath path = paths[iter];
            if (path != ProceduralGenerator.CurrentLayout &&
                path != ProceduralGenerator.PreviousLayout &&
                path.gameObject.activeInHierarchy &&
                pathAmount > value)
                path.gameObject.SetActive(false);

            pathAmount = iter;
        }
    }

    public void SubmitDistanceToManager()
    {
        GameManager.DetermineTiming(distance);
    }

    private void Init()
    {

        generator = transform.parent.parent.GetComponent<ProceduralGenerator>();

        

        //Set distance target
        distanceCheck.SetTarget(player.transform);


        generateEvent = EventManager.AddNewEvent(42, "generateLayout", () => SignalGeneration());

        //Set up OnRangeEnter Event
        distanceCheck.OnRangeEnter.AddNewListener(
        () =>
        {
            if (player != null && turningPoint)
                player.AllowTurn();

            if (!ProceduralGenerator.IsStalling)
            {
                SignalLayoutSpawn signal = layout.GetSignal();
                ProceduralGenerator.PreviousLayout = ProceduralGenerator.CurrentLayout;
                ProceduralGenerator.CurrentLayout = layout;
                ProceduralGenerator.FlushPaths();
                player.UpdateSignalPoint(signal);
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
            GameManager.ResetTime();
        });
    }

    /// <summary>
    /// Start of Object
    /// </summary>
    private void OnEnable()
    {
        player = GameManager.player;

        StartCoroutine(DistanceCheckingLoop());  
    }

}
