using System.Collections;
using UnityEngine;

public class PlayerPawn : Pawn
{
    /// <summary>
    /// Have the pawn move left
    /// </summary>
    public override void MoveLeft()
    {
        currentDirection = Direction.LEFT;
        transform.eulerAngles = new Vector3(0f, 0f, 90f);
    }

    /// <summary>
    /// Have the pawn move right
    /// </summary>
    public override void MoveRight()
    {
        currentDirection = Direction.RIGHT;
        transform.eulerAngles = new Vector3(0f, 0f, 270f);
    }

    /// <summary>
    /// Have the pawn move up
    /// </summary>
    public override void MoveUp()
    {
        currentDirection = Direction.UP;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    /// <summary>
    /// Have the pawn move down
    /// </summary>
    public override void MoveDown()
    {
        currentDirection = Direction.DOWN;
        transform.eulerAngles = new Vector3(0f, 0f, 180f);
    }

    // Unique to player
    public override void Boost()
    {
        GameManager.BurstIntoBoost();
    }
}
