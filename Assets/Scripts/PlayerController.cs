﻿using System;
using System.Collections;
using UnityEngine;

public class PlayerController : Controller
{
    [SerializeField]
    TouchDetection movementTouchArea, boostTouchArea;

    [SerializeField]
    Sensor[] sensors;

    PlayerPawn player;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Update()
    {
        if (GameManager.IsGameStarted)
        {
            InputControls();
            TouchInputControls();
        }
    }

    /// <summary>
    /// Initialize Player Controller
    /// </summary>
    void Init()
    {
        Keymapper.Configure(
            new Keymapper.Key("left", KeyCode.LeftArrow),
            new Keymapper.Key("right", KeyCode.RightArrow),
            new Keymapper.Key("up", KeyCode.UpArrow),
            new Keymapper.Key("down", KeyCode.DownArrow),
            new Keymapper.Key("boost", KeyCode.Space));

         player = AssociatedPawn as PlayerPawn;
    }

    void InputControls()
    {
        
        //Movement controls
        if (Keymapper.OnKeyDown("left") &&
            player.GetDirection() != Direction.RIGHT &&
            player.CanTurn())
        {
            player.MoveLeft();
            player.ProhibitTurn();
        }

        else if (Keymapper.OnKeyDown("right") &&
            player.GetDirection() != Direction.LEFT &&
            player.CanTurn())
        {
            player.MoveRight();
            player.ProhibitTurn();
        }

        else if (Keymapper.OnKeyDown("up") &&
            player.GetDirection() != Direction.DOWN &&
            player.CanTurn())
        {
            player.MoveUp();
            player.ProhibitTurn();
        }

        else if (Keymapper.OnKeyDown("down") &&
            player.GetDirection() != Direction.UP &&
            player.CanTurn())
        {
            player.MoveDown();
            player.ProhibitTurn();
        }

        //Boosting
        if (Keymapper.OnKeyDown("boost"))
        {
            player.Boost();
        }
    }

    void TouchInputControls()
    {
        PlayerPawn player = AssociatedPawn as PlayerPawn;
        //Movement controls
        if (movementTouchArea.SlideLeft() &&
            player.GetDirection() != Direction.RIGHT &&
            player.CanTurn())
        {
            player.MoveLeft();
            player.ProhibitTurn();
        }

        else if (movementTouchArea.SlideRight() &&
            player.GetDirection() != Direction.LEFT &&
            player.CanTurn())
        {
            player.MoveRight();
            player.ProhibitTurn();
        }

        else if (movementTouchArea.SlideUp() &&
            player.GetDirection() != Direction.DOWN &&
            player.CanTurn())
        {
            player.MoveUp();
            player.ProhibitTurn();
        }

        else if (movementTouchArea.SlideDown() &&
            player.GetDirection() != Direction.UP &&
            player.CanTurn())
        {
            player.MoveDown();
            player.ProhibitTurn();
        }

        //Boosting
        if (boostTouchArea.Tap())
        {
            player.Boost();
        }
    }
}
