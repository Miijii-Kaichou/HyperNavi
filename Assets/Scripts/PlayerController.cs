using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
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

    }

    IEnumerator InputCycle()
    {
        while (true)
        {
            try
            {
                InputControls();
            }
            catch(Exception e)
            {
                Debug.Log("Input Controls threw an exception: " + e.Message);
            }
            yield return null;
        }
    }
}
