using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPawn : Pawn
{
    [SerializeField]
    private WallDetector wallDetector;

    private bool hasContactedWall = false;

    private bool canTurn = false;

    private SignalLayoutSpawn signalPoint;

    private bool invincibility = false;

    private float time = 0f;
    private float duration = 0.5f;
    protected override void Start()
    {
        StartCoroutine(IFrameLoop());
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Have the pawn move left
    /// </summary>
    public override void MoveLeft()
    {
        currentDirection = Direction.LEFT;
        transform.eulerAngles = new Vector3(0f, 0f, 90f);
        signalPoint.SubmitDistanceToManager();
    }

    /// <summary>
    /// Have the pawn move right
    /// </summary>
    public override void MoveRight()
    {
        currentDirection = Direction.RIGHT;
        transform.eulerAngles = new Vector3(0f, 0f, 270f);
        signalPoint.SubmitDistanceToManager();
    }

    /// <summary>
    /// Have the pawn move up
    /// </summary>
    public override void MoveUp()
    {
        currentDirection = Direction.UP;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        signalPoint.SubmitDistanceToManager();
    }

    /// <summary>
    /// Have the pawn move down
    /// </summary>
    public override void MoveDown()
    {
        currentDirection = Direction.DOWN;
        transform.eulerAngles = new Vector3(0f, 0f, 180f);
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

    public void ApplyIFrames()
    {
        invincibility = true;
        hasContactedWall = false;
        time = 0f;
    }

    IEnumerator IFrameLoop()
    {
        while (true)
        {
            if (invincibility)
            {
                time += Time.deltaTime;
                if(time >= duration)
                {
                    time = 0f;
                    invincibility = false;
                }
            }
        }
    }

    /// <summary>
    /// Update the signal point that the player is at
    /// </summary>
    /// <param name="signalPoint"></param>
    public void UpdateSignalPoint(SignalLayoutSpawn signalPoint)
    {
        this.signalPoint = signalPoint;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        try
        {
            hasContactedWall = collision.collider.GetType() == typeof(TilemapCollider2D);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}
