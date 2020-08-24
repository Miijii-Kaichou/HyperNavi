using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SignalLayoutSpawn : MonoBehaviour
{
    ProceduralGenerator generator;

    [SerializeField]
    OpeningPath layout;

    private void OnEnable()
    {
        generator = transform.parent.parent.GetComponent<ProceduralGenerator>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Signal generator to generate a layout based on the player's direction
        try {
            PlayerPawn player = collision.GetComponent<PlayerPawn>();
            if (player != null)
            {
                Side side = default;

                generator.previousLayout = generator.currentLayout;

                if (!generator.dontDeactivate && generator.previousLayout != null)
                    generator.previousLayout.gameObject.SetActive(false);

                generator.currentLayout = layout;

                generator.dontDeactivate = false;

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
        catch(Exception e)
        {
            //Do nothing
            Debug.LogError("If you get this error for some reason, please check it out...: " + e.Message);
        }
    }
}
