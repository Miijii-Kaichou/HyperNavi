using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    [SerializeField]
    protected Controller pawnController;

    protected Direction currentDirection = Direction.UP;
    protected Rigidbody2D rb2d;

    

    Vector3 setEulerAngle;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Vector2 direction = new Vector2(0f, GameManager.CurrentSpeed * (1 + GameManager.BoostSpeed));
        transform.Translate(direction * Time.deltaTime);
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
        currentDirection = Direction.LEFT;
        transform.eulerAngles = new Vector3(0f, 0f, 90f);
    }

    /// <summary>
    /// Have the pawn move right
    /// </summary>
    public virtual void MoveRight()
    {
        currentDirection = Direction.RIGHT;
        transform.eulerAngles = new Vector3(0f, 0f, 270f);
    }

    /// <summary>
    /// Have the pawn move up
    /// </summary>
    public virtual void MoveUp()
    {
        currentDirection = Direction.UP;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    /// <summary>
    /// Have the pawn move down
    /// </summary>
    public virtual void MoveDown()
    {
        currentDirection = Direction.DOWN;
        transform.eulerAngles =  new Vector3(0f, 0f, 180f);
    }

    /// <summary>
    /// Enter into a burst of speed
    /// </summary>
    public virtual void Boost()
    {

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
    private void Init()
    {
        rb2d = GetComponent<Rigidbody2D>();
        HookToController(pawnController);
        setEulerAngle = transform.eulerAngles;
    }
}
