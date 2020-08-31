using System.Collections;
using UnityEngine;

public class FollowBehind : MonoBehaviour
{
    //An object will follow it's target smoothly
    //Mainly for Raven and Maple's Emitters

    [SerializeField]
    float dampTime = 0.15f;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    Transform targetTranform;

    [SerializeField, Range(-10f, 10f)]
    float zDepth = 10f;

    private Vector2 initialPosition;

    Vector3 velocity = Vector2.zero;
    Vector3 point, delta, destination;

    private void Start()
    {
        StartCoroutine(FollowCycle());
        Init();
    }

    private void Init()
    {
        initialPosition = GetComponent<Transform>().position;
    }

    /// <summary>
    /// Main Camera Follow Cycle
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowCycle()
    {
        while (true)
        {
            if (targetTranform && initialPosition != null)
            {
                point = transform.localPosition - new Vector3(offset.x * Mathf.Sign(targetTranform.localScale.x), offset.y);
                delta = targetTranform.localPosition - new Vector3(point.x, point.y, zDepth);
                destination = transform.localPosition + delta;

                //Z will be changed
                transform.localPosition = Vector3.SmoothDamp(
                    new Vector3(transform.localPosition.x, transform.localPosition.y, -10), 
                    new Vector3(destination.x, destination.y, -10), 
                    ref velocity, dampTime);
            }
            yield return null;
        }
    }
    /// <summary>
    /// Attach a target to follow behind object
    /// </summary>
    /// <param name="target"></param>
    public void AttachTarget(Transform target) => targetTranform = target;

    /// <summary>
    /// Detach a target from follow behind object
    /// </summary>
    public void DetachTarget() => targetTranform = null; 

    public void SetOffSet(Vector2 offset)
    {
        this.offset = offset;
    }
}
