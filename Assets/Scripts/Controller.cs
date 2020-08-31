using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    protected Pawn AssociatedPawn { get; private set; }

    /// <summary>
    /// Assigns a pawn to become the associated pawn that the controller targets.
    /// </summary>
    /// <param name="pawn"></param>
    public virtual void AssignPawn(Pawn pawn)
    {
        AssociatedPawn = pawn;
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void Init()
    {

    }
}
