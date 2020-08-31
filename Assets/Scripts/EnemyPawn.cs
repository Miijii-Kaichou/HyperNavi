using System.Collections;
using UnityEngine;
public class EnemyPawn : Pawn
{
    protected override void Init()
    {
        rb2d = GetComponent<Rigidbody2D>();
        HookToController(pawnController);

        StartCoroutine(MovementCycle());
    }

    protected override IEnumerator MovementCycle()
    {
        while (true)
        {
            speed = GameManager.CurrentSpeed;
            direction = new Vector2(speed * xDir, speed * yDir) * Time.deltaTime;
            transform.localPosition += (Vector3)direction;
            yield return null;
        }
    }

    /// <summary>
    /// Have the pawn move left
    /// </summary>
    public override void MoveLeft()
    {
        currentDirection = Direction.LEFT;
        SetDirection(Sign.Negative, Sign.Zero);
    }

    /// <summary>
    /// Have the pawn move right
    /// </summary>
    public override void MoveRight()
    {
        currentDirection = Direction.RIGHT;
        SetDirection(Sign.Positive, Sign.Zero);
    }

    /// <summary>
    /// Have the pawn move up
    /// </summary>
    public override void MoveUp()
    {
        currentDirection = Direction.UP;
        SetDirection(Sign.Zero, Sign.Positive);
    }

    /// <summary>
    /// Have the pawn move down
    /// </summary>
    public override void MoveDown()
    {
        currentDirection = Direction.DOWN;
        SetDirection(Sign.Zero, Sign.Negative);
    }
}
