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

    private void Awake()
    {
        obj = GetComponent<IRange>();
    }

    private void Start()
    {
        if (target == null)
            target = GameManager.player.transform;
        InitializeEvents();
    }

    private void OnReset()
    {
        StartCoroutine(CheckDistance());
    }

    private void OnEnable()
    {
        OnReset();
    }

    IEnumerator CheckDistance()
    {
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

            if (onEnter.HasListerners())
                onEnter.Trigger();
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

            if (onExit.HasListerners())
                onExit.Trigger();

        }
    }


    /// <summary>
    /// Calculate distance between point a and b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    void CalculateDistance(Transform a, Transform b)
    {
        Distance = Vector2.Distance(a.position, b.position);
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
