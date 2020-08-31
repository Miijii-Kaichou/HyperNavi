using System.Collections;
using UnityEngine;

public abstract class Pawn : EnhancedMono
{
    [SerializeField]
    protected Controller pawnController;

    protected Direction currentDirection = Direction.UP;
    protected Rigidbody2D rb2d;

    protected private float speed;
    protected Vector2 direction;

    protected int xDir = 0, yDir = 1;

    protected IEnumerator movementCycle;

    private void Awake()
    {
        movementCycle = MovementCycle();
    }

    protected virtual IEnumerator MovementCycle()
    {
        while (true)
        {
            speed = GameManager.CurrentSpeed * (1 + GameManager.BoostSpeed);
            direction = new Vector2(speed * xDir, speed * yDir) * Time.deltaTime;
            transform.localPosition += (Vector3)direction;
            yield return null;
        }
    }

    protected virtual void OnEnable()
    {
        Init();
    }

    /// <summary>
    /// Have the pawn move left
    /// </summary>
    public virtual void MoveLeft()
    {
  
    }

    /// <summary>
    /// Have the pawn move right
    /// </summary>
    public virtual void MoveRight()
    {

    }

    /// <summary>
    /// Have the pawn move up
    /// </summary>
    public virtual void MoveUp()
    {

    }

    /// <summary>
    /// Have the pawn move down
    /// </summary>
    public virtual void MoveDown()
    {

        
    }

    /// <summary>
    /// Enter into a burst of speed
    /// </summary>
    public virtual void Boost()
    {

    }

    public virtual void SetDirection(int xSign, int ySign)
    {
        xDir = xSign;
        yDir = ySign;
    }

    /// <summary>
    /// Get the current direction of the player
    /// </summary>
    /// <returns></returns>
    public virtual Direction GetDirection() => currentDirection;

    /// <summary>
    /// Hooks a pawn to a controller in order to be controlled
    /// </summary>
    /// <param name="controller"></param>
    protected virtual void HookToController(Controller controller)
    {
        controller.AssignPawn(this);
    }

    /// <summary>
    /// Initialize Pawn
    /// </summary>
    protected virtual void Init()
    {
        
    }
}
