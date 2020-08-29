using System.Collections;
using UnityEngine;

public class DistanceCheck : MonoBehaviour
{
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
            }

            yield return new WaitForFixedUpdate();
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
