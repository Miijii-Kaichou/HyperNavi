using System.Collections;
using UnityEngine;
using Event = EventManager.Event;

public class DistanceCheck : MonoBehaviour
{
    [SerializeField]
    float range = 2f;

    [SerializeField]
    private float Distance = 0f;

    [SerializeField]
    Transform target = null;

    Event onEnter, onExit;

    bool inside = false;

    IRange obj;

    //Coroutines
    IEnumerator checkDistanceCycle;

    private void Awake()
    {
        checkDistanceCycle = CheckDistance();
        obj = GetComponent<IRange>();
    }

    private void Start()
    {
       
        InitializeEvents();
    }

    private void OnReset()
    {
        StartCoroutine(checkDistanceCycle);
    }

    private void OnEnable()
    {
        OnReset();
    }

    IEnumerator CheckDistance()
    {
        if (target == null)
            target = GameManager.player.transform;

        while (true)
        {
            if (target != null)
            {
                CalculateDistance(target, transform);

                CheckIfInRange();
                CheckIfExitFromRange();
            }

            yield return null;
        }
    }

    /// <summary>
    /// Check if player is within range
    /// </summary>
    private void CheckIfInRange()
    {
        if (Distance <= range && !inside)
        {
            inside = true;

            if (onEnter != null && onEnter.HasListerners())
                onEnter.Trigger();

            return;
        }
    }

    /// <summary>
    /// Check if player is now out of range
    /// </summary>
    private void CheckIfExitFromRange()
    {
        if (Distance > range && inside)
        {
            inside = false;

            if (onExit != null && onExit.HasListerners())
                onExit.Trigger();
            return;
        }
    }


    /// <summary>
    /// Calculate distance between point a and b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    void CalculateDistance(Transform a, Transform b)
    {
        Distance = Mathf.Sqrt(
            Mathf.Pow(b.position.x - a.position.x,2f) + 
            Mathf.Pow(b.position.y - a.position.y,2f)
            );

        return;
    }

    public float GetDistance() => Distance;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void InitializeEvents()
    {
        onEnter = EventManager.AddNewEvent(EventManager.FreeValue, "onEnter", () => obj.OnRangeEnter());
        onExit = EventManager.AddNewEvent(EventManager.FreeValue, "onExit", () => obj.OnRangeExit());
    }
}
