using UnityEngine;

public class Sensor : MonoBehaviour
{
    Collider2D collision;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.collision = collision;
    }
}
