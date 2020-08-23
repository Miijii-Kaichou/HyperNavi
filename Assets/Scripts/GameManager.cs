using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public class GameManager : MonoBehaviour
{
    
    private static GameManager Instance;
    public GameManager instance {
        get
        {
            return Instance;
        }

        private set
        {
            if(Instance == null)
            {
                instance = this;
                Instance = instance;
                DontDestroyOnLoad(Instance);
            }
        }
    }

    // Handles all gaming things in the game

    // The starting speed of the player
    public static float InitialSpeed { get; private set; } = 5f;

    public static float SpeedRate { get; private set; } = 0.1f;

    public static float CurrentSpeed { get; private set; } = InitialSpeed;

    public static bool IsGameStarted { get; private set; } = false;

    public static int CurrentScore { get; private set; } = 0;

    // The max speed of the player
    public const float MaxSpeed = 30f;

    public const float FULL_SEC = 1;

    public static PlayerPawn player;

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    // Increase the player speed over time;
    static void Accelerate()
    {
 
        CurrentSpeed += SpeedRate;
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, InitialSpeed, MaxSpeed);
    }

    static IEnumerator AccelerationCycle()
    {
        while(true)
        {
            // Increase Speed;
            Accelerate();

            //Every quarter of a second, we'll increase speed
            yield return new WaitForSeconds(FULL_SEC / 4);
        }
    }

    /// <summary>
    /// Start the game officially
    /// </summary>
    public void StartGame()
    {
        IsGameStarted = true;
        StartCoroutine(AccelerationCycle());
    }
}
