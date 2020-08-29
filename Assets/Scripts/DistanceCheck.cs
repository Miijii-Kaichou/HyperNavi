using System.Collections;
using UnityEngine;

public class DistanceCheck : MonoBehaviour
{
    [SerializeField]
    SignalLayoutSpawn signal;

    [SerializeField]
    private float Distance = 0f;

    Transform target = null;

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
        if (Distance <= signal.range && !signal.inside)
        {
            signal.inside = true;
            signal.OnSignalInteraction();
        }
    }

    /// <summary>
    /// Check if player is now out of range
    /// </summary>
    private void CheckIfExitFromRange()
    {
        if (Distance > signal.range && signal.inside)
        {
            signal.inside = false;
            signal.OnEndSignalInteraction();
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
}
