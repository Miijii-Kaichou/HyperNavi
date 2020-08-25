using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCheck : MonoBehaviour
{
    [SerializeField]
    private float radius;
    public float Distance { get; private set; } = 0f;

    public EventManager.Event OnRangeEnter { get; set; }
    public EventManager.Event OnRangeStay { get; set; }
    public EventManager.Event OnRangeExit { get; set; }

    Transform target = null;

    bool trigger = false;
    bool inside = false;

    float time = 0;

    private void Start()
    {
        StartCoroutine(DistanceCheckLoop());
        StartCoroutine(DeadTime());
    }

    /// <summary>
    /// Calculate distance between point a and b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    void CalculateDistance(Transform a, Transform b)
    {
        Distance = Vector3.Distance(b.position, a.position);
    }

    IEnumerator DistanceCheckLoop()
    {
        while (true)
        {
            CalculateDistance(transform, target);

            if (!trigger && !inside && Distance <= radius)
                trigger = true;

            if (Distance <= radius && trigger)
            {
                inside = true;
                OnRangeStay.Trigger();
            }

            if (inside && Distance > radius)
            {
                inside = false;
                trigger = false;
                OnRangeExit.Trigger();
            }


            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DeadTime()
    {
        while (true)
        {
            if (trigger)
            {
                OnRangeEnter.Trigger();
                trigger = false;
            }
            yield return null;
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
