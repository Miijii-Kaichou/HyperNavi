using System.Collections;
using UnityEngine;

public class WallDetector : MonoBehaviour
{
    [SerializeField]
    private float detectionDistance = 0;

    private bool hasContactedWall = false;

    private RaycastHit2D hit;

    private Vector2 pos;

    private LayerMask layoutMask;

    private void Awake()
    {
        layoutMask = 1 << 9;
    }

    private void OnEnable()
    {
        StartCoroutine(WallDetectionLoop());
    }

    IEnumerator WallDetectionLoop()
    {
        while (true)
        {
            pos = transform.position;
            Debug.DrawRay(pos, transform.up * detectionDistance, Color.red);
            hit = Physics2D.Raycast(pos, transform.up, detectionDistance, layoutMask);
            hasContactedWall = hit.collider != null;
            yield return null;
        }
    }

    public bool HasCollided() => hasContactedWall;

    private void OnDisable()
    {
        hasContactedWall = false;
    }
}
