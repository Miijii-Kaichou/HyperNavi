using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCheck : MonoBehaviour
{
    [SerializeField]
    private float radius;

    public float Distance = 0f;

    public EventManager.Event OnRangeEnter;
    public EventManager.Event OnRangeStay;
    public EventManager.Event OnRangeExit;

    Transform target = null;

    bool trigger = false;
    bool inside = false;

    float time = 0;

    private void Awake()
    {
        OnRangeEnter = EventManager.AddNewEvent(32, "onRangeEnter");
        OnRangeStay = EventManager.AddNewEvent(33, "onRangeStay");
        OnRangeExit = EventManager.AddNewEvent(34, "onRangeExit");
    }

    private void OnEnable()
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
        Distance = Vector3.Distance(b.localPosition, a.localPosition);
    }

    IEnumerator DistanceCheckLoop()
    {
        while (true)
        {
            
            if (target != null && enabled)
            {
                CalculateDistance(target, transform.parent);

                if (!trigger && !inside && Distance <= radius)
                    trigger = true;

                if (Distance <= radius)
                {
                    try
                    {
                        inside = true;
                        if (OnRangeStay.HasListerners())
                            OnRangeStay.Trigger();
                    }
                    catch { }
                }

                if (inside && Distance > radius)
                {
                    inside = false;
                    if ( OnRangeExit.HasListerners())
                        OnRangeExit.Trigger();
                }
            }

            yield return null;
        }
    }

    IEnumerator DeadTime()
    {
        while (true)
        {
            if (trigger && OnRangeEnter != null && OnRangeEnter.HasListerners())
            {
                OnRangeEnter.Trigger();
                OnRangeEnter.Reset();
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
