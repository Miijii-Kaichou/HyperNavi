using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallDetector : MonoBehaviour
{
    private bool hasContactedWall = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        try
        {
            TilemapCollider2D tilemap = collision.GetComponent<TilemapCollider2D>();
            hasContactedWall = tilemap != null;
            Debug.Log(collision);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public bool HasCollided() => hasContactedWall;
}
