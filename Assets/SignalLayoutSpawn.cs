using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalLayoutSpawn : MonoBehaviour
{
    ProceduralGenerator generator;

    [SerializeField]
    OpeningPath layout;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        generator = transform.parent.parent.GetComponent<ProceduralGenerator>();
        Debug.Log(generator != null ? "We got it!" : "We don't got it. Try again.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Signal generator to generate a layout based on the player's direction
        try {
            PlayerPawn player = collision.GetComponent<PlayerPawn>();
            if (player != null)
            {
                Side side = default;
                generator.previousLayout = generator.currentLayout;
                generator.currentLayout = layout;
                switch (player.GetDirection())
                {
                    case Direction.LEFT:
                        //Coming from left. Need right to open
                        side = Side.RIGHT;
                        generator.GenerateLayout(side, layout);
                        break;

                    case Direction.RIGHT:
                        //Coming from Right. Need left to open
                        side = Side.LEFT;
                        generator.GenerateLayout(side, layout);
                        break;

                    case Direction.UP:
                        //Coming from top. Need bottome to open
                        side = Side.BOTTOM;
                        generator.GenerateLayout(side, layout);
                        break;

                    case Direction.DOWN:
                        //Coming from bottom. Need top to open
                        side = Side.TOP;
                        generator.GenerateLayout(side, layout);
                        break;

                    default:
                        break;
                }

                
            }
        }
        catch
        {
            //Do nothing
        }
    }
}
