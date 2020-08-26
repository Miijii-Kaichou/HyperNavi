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
        layoutMask = LayerMask.GetMask("Layout");
    }

    private void Start()
    {
        StartCoroutine(WallDetectionLoop());
    }

    IEnumerator WallDetectionLoop()
    {
        while (true)
        {
            pos = transform.position;
            hit = Physics2D.Raycast(pos, transform.up, detectionDistance, layoutMask);
            hasContactedWall = hit.collider != null;
            yield return null;
        }
    }

    public bool HasCollided() => hasContactedWall;
}
