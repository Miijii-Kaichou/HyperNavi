using System.Collections;
using UnityEngine;

public class PlayerPawn : Pawn
{
    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    private WallDetector wallDetector;

    [SerializeField]
    private bool hasContactedWall = false;

    private bool canTurn = false;

    private SignalLayoutSpawn signalPoint;

    private bool iFramesOn = false;

    private float time = 0;


    protected override void OnEnable()
    {
        Init();
    }

    protected override void Init()
    {
        rb2d = GetComponent<Rigidbody2D>();
        HookToController(pawnController);

        StartCoroutines(
            MovementCycle(),
            WallDetectionLoop(),
            IFrames());
    }

    IEnumerator IFrames()
    {
        while (true)
        {
            if (iFramesOn)
            {
                time += Time.deltaTime;
                if (time >= 1f)
                {
                    iFramesOn = false;
                    time = 0f;
                }
            }
            yield return null;
            ;
        }
    }

    IEnumerator WallDetectionLoop()
    {
        while (true)
        {
            hasContactedWall = !iFramesOn && wallDetector.HasCollided();

            yield return null;
            ;
        }
    }

    /// <summary>
    /// Have the pawn move left
    /// </summary>
    public override void MoveLeft()
    {
        signalPoint.SubmitDistanceToManager();
        currentDirection = Direction.LEFT; ;
        SetDirection(Sign.Negative, Sign.Zero); ;
        transform.eulerAngles = new Vector3(0, 0, 90f);
    }

    /// <summary>
    /// Have the pawn move right
    /// </summary>
    public override void MoveRight()
    {
        signalPoint.SubmitDistanceToManager();
        currentDirection = Direction.RIGHT;

        transform.eulerAngles = new Vector3(0, 0, 270);
        SetDirection(Sign.Positive, Sign.Zero);
    }

    /// <summary>
    /// Have the pawn move up
    /// </summary>
    public override void MoveUp()
    {
        signalPoint.SubmitDistanceToManager();
        currentDirection = Direction.UP;

        transform.eulerAngles = new Vector3(0, 0, 0);
        SetDirection(Sign.Zero, Sign.Positive);
    }

    /// <summary>
    /// Have the pawn move down
    /// </summary>
    public override void MoveDown()
    {
        signalPoint.SubmitDistanceToManager();
        currentDirection = Direction.DOWN;

        transform.eulerAngles = new Vector3(0, 0, 180);
        SetDirection(Sign.Zero, Sign.Negative);
    }

    // Unique to player
    public override void Boost()
    {
        GameManager.BurstIntoBoost();
    }

    /// <summary>
    /// Check if player has collided with wall
    /// </summary>
    /// <returns></returns>
    public bool HasContactedWall() => hasContactedWall;

    /// <summary>
    /// Check if player can turn
    /// </summary>
    /// <returns></returns>
    public bool CanTurn() => canTurn;

    /// <summary>
    /// Allow the player to turn into a different direction
    /// </summary>
    public void AllowTurn() => canTurn = true;

    /// <summary>
    /// Prohibit the player from turning
    /// </summary>
    public void ProhibitTurn() => canTurn = false;

    public Rigidbody2D GetRigidbody() => rb2d;

    public void ChangeDirection(Direction newDirection) => currentDirection = newDirection;

    /// <summary>
    /// Update the signal point that the player is at
    /// </summary>
    /// <param name="signalPoint"></param>
    public void UpdateSignalPoint(SignalLayoutSpawn signalPoint)
    {
        this.signalPoint = signalPoint;
    }

    public SignalLayoutSpawn GetLastSignalPoint() => signalPoint;

    /// <summary>
    /// Get player pawn's controller
    /// </summary>
    /// <returns></returns>
    public PlayerController GetController() => controller;

    public void EnableIFrame() => iFramesOn = true;

    public void SetXDir(int sign)
    {
        xDir = (int)Mathf.Sign(sign);
    }

    public void SetYDir(int sign)
    {
        yDir = (int)Mathf.Sign(sign);
    }
}
