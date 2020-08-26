﻿using System.Collections;
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
    Transform parent;

    bool trigger = false;
    bool inside = false;

    EventManager.Event rerunCoroutineEvent;

    private void Awake()
    {
        parent = transform.parent;
        OnRangeEnter = EventManager.AddNewEvent(32, "onRangeEnter");
        OnRangeStay = EventManager.AddNewEvent(33, "onRangeStay");
        OnRangeExit = EventManager.AddNewEvent(34, "onRangeExit");

        rerunCoroutineEvent = EventManager.AddNewEvent(35, "resetCoroutine", () =>
        {
            StartCoroutine(CheckDistance());
            StartCoroutine(DeadTime());
        });
    }

    private void OnReset()
    {
        rerunCoroutineEvent.Trigger();
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
                CalculateDistance(ref target, ref parent);

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
                    if (OnRangeExit.HasListerners())
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

    /// <summary>
    /// Calculate distance between point a and b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    void CalculateDistance(ref Transform a, ref Transform b)
    {
        Distance = Vector3.Distance(b.localPosition, a.localPosition);
        return;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
