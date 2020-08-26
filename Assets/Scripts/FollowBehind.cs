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

    Camera m_camera;

    private void Start()
    {
        StartCoroutine(CameraFollowCycle());
        Init();
    }

    private void Init()
    {
        initialPosition = GetComponent<Transform>().position;
        m_camera = GetComponent<Camera>();
    }


    IEnumerator CameraFollowCycle()
    {
        while (true)
        {
            if (targetTranform && m_camera != null && initialPosition != null)
            {
                Vector3 point = transform.localPosition - new Vector3(offset.x * Mathf.Sign(targetTranform.localScale.x), offset.y);
                Vector3 delta = targetTranform.localPosition - new Vector3(point.x, point.y, zDepth);
                Vector3 destination = transform.localPosition + delta;

                //Z will be changed
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, destination, ref velocity, dampTime);
            }
            yield return null;
        }
    }

    public void AssignTarget(Transform target) => targetTranform = target;
}
