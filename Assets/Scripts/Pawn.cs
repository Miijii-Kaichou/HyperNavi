﻿using System.Collections;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    [SerializeField]
    protected Controller pawnController;

    protected Direction currentDirection = Direction.UP;
    protected Rigidbody2D rb2d;

    private float speed;
    Vector2 direction;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        StartCoroutine(MovementCycle());    
    }

    IEnumerator MovementCycle()
    {
        while (true)
        {
            speed = GameManager.CurrentSpeed * (1 + GameManager.BoostSpeed);
            direction = new Vector2(0f, speed) * Time.deltaTime;
            transform.Translate(direction);
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
    }
}
