using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

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

    // Handles all gaming things in the game

    //Const for QuarterOfSec
    const float QUARTER_OF_A_SEC = 0.25f;

    /// <summary>
    /// The starting speed of the player
    /// </summary>
    public static float InitialSpeed { get; private set; } = 500f;

    /// <summary>
    /// The rate in which to increase speed
    /// </summary>
    public static float SpeedRate { get; private set; } = 0.5f;

    /// <summary>
    /// The speed added to current speed over a certain rate
    /// </summary>
    public static float SpeedAcceleration { get; private set; } = 0.001f;

    /// <summary>
    /// Current speed of the player
    /// </summary>
    public static float CurrentSpeed { get; private set; } = InitialSpeed;

    /// <summary>
    /// Check if the game has started
    /// </summary>
    public static bool IsGameStarted { get; private set; } = false;

    /// <summary>
    /// Current score
    /// </summary>
    public static int CurrentScore { get; private set; } = 0;

    //This speed is added to the current speed
    public static float BoostSpeed { get; private set; } = 0f;

    /// <summary>
    /// This is the burst value of the boost. This will be assigned to the BoostSpeed
    /// BoostSpeed will then gradually lerp back to zero
    /// </summary>
    public static float BoostBurstValue { get; private set; } = 5f;

    /// <summary>
    /// The rate the boost speed will deplete in seconds
    /// </summary>
    public static float BoostSlowdownRate { get; private set; } = 0.025f;

    /// <summary>
    ///  How much BoostSpeed you'll loose over a certain rate
    /// </summary>
    public static float BoostDepletionValue { get; private set; } = 0.1f;

    /// <summary>
    /// The max speed of the player
    /// </summary>
    public const float MaxSpeed = 30f;

    /// <summary>
    /// A full second
    /// </summary>
    public const float FULL_SEC = 1;

    public static PlayerPawn player;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }    else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Test Again
        //StartGame();
    }

    /// <summary>
    /// Start the game officially
    /// </summary>
    public void StartGame()
    {
        IsGameStarted = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPawn>();
        StartCoroutine(GameLoop());
    }

    /// <summary>
    /// Begin boost
    /// </summary>
    public static void BurstIntoBoost()
    {
        BoostSpeed = BoostBurstValue;
    }

    public static void DetermineTiming()
    {

    }

    IEnumerator Friction()
    {
        while (true)
        {
            BoostSpeed = Mathf.Lerp(BoostSpeed, 0f, BoostDepletionValue);
            yield return new WaitForSeconds(BoostSlowdownRate);
        }
    }

    IEnumerator Accelerate()
    {
        while (true)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, SpeedAcceleration);
            yield return new WaitForSeconds(SpeedRate);
        }
    }

    IEnumerator GameLoop()
    {
        //Start friction
        StartCoroutine(Friction());

        //Start acceleration
        StartCoroutine(Accelerate());

        while (true)
        {
            if (player != null && player.HasContactedWall())
                player.gameObject.SetActive(false);

            yield return null;
        }
    }
}
