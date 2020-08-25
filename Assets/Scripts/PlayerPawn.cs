using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPawn : Pawn
{
    [SerializeField]
    private WallDetector wallDetector;

    [SerializeField]
    private float gridSize = 1f;

    [SerializeField]
    private bool hasContactedWall = false;

    private bool canTurn = false;

    private SignalLayoutSpawn signalPoint;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        hasContactedWall = wallDetector.HasCollided();
        base.Update();
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

    /// <summary>
    /// Update the signal point that the player is at
    /// </summary>
    /// <param name="signalPoint"></param>
    public void UpdateSignalPoint(SignalLayoutSpawn signalPoint)
    {
        this.signalPoint = signalPoint;
    }
}
