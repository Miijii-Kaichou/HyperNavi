using System;
using System.Collections;
using UnityEngine;

public class PlayerController : Controller
{
    [SerializeField]
    TouchDetection movementTouchArea, boostTouchArea;

    [SerializeField]
    Sensor[] sensors;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        //Start getting input from player
        StartCoroutine(InputCycle());
    }

    void InputControls()
    {
        //Movement controls
        if (Keymapper.OnKeyDown("left")){
            AssociatedPawn.MoveLeft();
        }

        else if (Keymapper.OnKeyDown("right") )
        {
            AssociatedPawn.MoveRight();
        }

        else if (Keymapper.OnKeyDown("up"))
        {
            AssociatedPawn.MoveUp();
        }

        else if (Keymapper.OnKeyDown("down"))
        {
            AssociatedPawn.MoveDown();
        }

        //Boosting
        if (Keymapper.OnKeyDown("boost"))
        {
            AssociatedPawn.Boost();
        }
    }

    void TouchInputControls()
    {
        //Movement controls
        if (movementTouchArea.SlideLeft())
        {
            AssociatedPawn.MoveLeft();
        }

        else if (movementTouchArea.SlideRight())
        {
            AssociatedPawn.MoveRight();
        }

        else if (movementTouchArea.SlideUp())
        {
            AssociatedPawn.MoveUp();
        }

        else if (movementTouchArea.SlideDown())
        {
            AssociatedPawn.MoveDown();
        }

        //Boosting
        if (boostTouchArea.Tap())
        {
            AssociatedPawn.Boost();
        }
    }

    IEnumerator InputCycle()
    {
        while (true)
        {
            try
            {
                InputControls();
                TouchInputControls();
            }
            catch(Exception e)
            {
                Debug.LogError("Input Controls threw an exception: " + e.Message);
            }
            yield return null;
        }
    }
}
