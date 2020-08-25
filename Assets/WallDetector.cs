using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallDetector : MonoBehaviour
{
    [SerializeField]
    private float detectionDistance = 0;
    private bool hasContactedWall = false;

    private void Update()
    {
        RaycastHit2D hit;
        Vector2 pos = transform.position;
        Debug.DrawRay(transform.position, Vector2.up * detectionDistance, Color.red);
        hit = Physics2D.Raycast(pos, Vector2.up, detectionDistance);
        Debug.Log(hit.collider);
        hasContactedWall = hit.collider != null && hit.collider.GetType() == typeof(TilemapCollider2D);

        
        if (hasContactedWall)
            Debug.Log("We hit da wall!!!");
    }

    public bool HasCollided() => hasContactedWall;
}
