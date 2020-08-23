using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    Direction currentDirection = default;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Have the pawn move left
    /// </summary>
    protected virtual void MoveLeft()
    {
        currentDirection = Direction.LEFT;
    }

    /// <summary>
    /// Have the pawn move right
    /// </summary>
    protected virtual void MoveRight()
    {
        currentDirection = Direction.RIGHT;
    }

    /// <summary>
    /// Have the pawn move up
    /// </summary>
    protected virtual void MoveUp()
    {
        currentDirection = Direction.UP;
    }

    /// <summary>
    /// Have the pawn move down
    /// </summary>
    protected virtual void MoveDown()
    {
        currentDirection = Direction.DOWN;
    }

    protected virtual HookToController(Controller controller)
    {
       
    }
}
