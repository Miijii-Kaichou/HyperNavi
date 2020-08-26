using System.Collections;
using UnityEngine;

public class PlayerPawn : Pawn
{
    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    private WallDetector wallDetector;

    [SerializeField]
    private float gridSize = 1f;

    [SerializeField]
    private bool hasContactedWall = false;

    private bool canTurn = false;

    private SignalLayoutSpawn signalPoint;


    protected override void OnEnable()
    {
        StartCoroutine(WallDetectionLoop());
        base.OnEnable();
    }

    IEnumerator WallDetectionLoop()
    {
        while (true)
        {
            hasContactedWall = wallDetector.HasCollided();

            yield return null;
        }
    }

    /// <summary>
    /// Have the pawn move left
    /// </summary>
    public override void MoveLeft()
    {
        currentDirection = Direction.LEFT;
        transform.eulerAngles = new Vector3(0, 0, 90);
        signalPoint.SubmitDistanceToManager();
    }

    /// <summary>
    /// Have the pawn move right
    /// </summary>
    public override void MoveRight()
    {
        currentDirection = Direction.RIGHT;
        transform.eulerAngles = new Vector3(0, 0, 270);
        signalPoint.SubmitDistanceToManager();
    }

    /// <summary>
    /// Have the pawn move up
    /// </summary>
    public override void MoveUp()
    {
        currentDirection = Direction.UP;
        transform.eulerAngles = new Vector3(0, 0, 0);
        signalPoint.SubmitDistanceToManager();
    }

    /// <summary>
    /// Have the pawn move down
    /// </summary>
    public override void MoveDown()
    {
        currentDirection = Direction.DOWN;
        transform.eulerAngles = new Vector3(0, 0, 180);
        signalPoint.SubmitDistanceToManager();
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
    public void UpdateSignalPoint(ref SignalLayoutSpawn signalPoint)
    {
        this.signalPoint = signalPoint;
    }

    /// <summary>
    /// Get player pawn's controller
    /// </summary>
    /// <returns></returns>
    public PlayerController GetController() => controller;
}
