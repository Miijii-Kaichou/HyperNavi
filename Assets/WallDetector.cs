using UnityEngine;

public class WallDetector : MonoBehaviour
{
    [SerializeField]
    private float detectionDistance = 0;
    private bool hasContactedWall = false;

    private void Update()
    {
        RaycastHit2D hit;
        Vector2 pos = transform.position;
        Debug.DrawRay(transform.position, transform.up * detectionDistance, Color.red);
        hit = Physics2D.Raycast(pos, transform.up, detectionDistance, LayerMask.GetMask("Layout"));
        hasContactedWall = hit.collider != null;
    }

    public bool HasCollided() => hasContactedWall;
}
