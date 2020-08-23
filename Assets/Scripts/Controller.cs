using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    protected Pawn AssociatedPawn { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Assigns a pawn to become the associated pawn that the controller targets.
    /// </summary>
    /// <param name="pawn"></param>
    public virtual void AssignPawn(Pawn pawn)
    {
        AssociatedPawn = pawn;
    }
}
