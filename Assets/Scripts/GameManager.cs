﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public static float InitialSpeed { get; private set; } = 5f;

    /// <summary>
    /// The rate in which to increase speed
    /// </summary>
    public static float SpeedRate { get; private set; } = 0.5f;

    /// <summary>
    /// The speed added to current speed over a certain rate
    /// </summary>
    public static float SpeedAcceleration { get; private set; } = 0.0005f;

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

    public const int RESET = 0;

    public static PlayerPawn player;

    public static bool dontDestroy = false;

    private static float time;
    private static float deadTime = 0.1f;

    private const int DEFAULT_FRAMERATE = 60;

    private static GameObject playerDeathObj;

    private void Awake()
    {
        Application.targetFrameRate = DEFAULT_FRAMERATE;

        //Load Title Screen
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Additive);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPawn>();
    }

    /// <summary>
    /// Start the game officially
    /// </summary>
    public static void StartGame()
    {
        IsGameStarted = true;

        //Start Game loop
        Instance.StartCoroutine(GameLoop());
    }

    /// <summary>
    /// Begin boost
    /// </summary>
    public static void BurstIntoBoost()
    {
        BoostSpeed = BoostBurstValue;
    }

    public static void DetermineTiming(ref float distance)
    {
        dontDestroy = (distance < 2f);
    }

    public static void AllowDestructionOfPlayer() => dontDestroy = false;

    public static void ResetTime()
    {
        time = RESET;
    }

    static IEnumerator Friction()
    {
        while (true)
        {
            BoostSpeed = Mathf.Lerp(BoostSpeed, 0f, BoostDepletionValue);
            yield return new WaitForSeconds(BoostSlowdownRate);
        }
    }

    static IEnumerator Accelerate()
    {
        while (true)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, SpeedAcceleration);
            yield return new WaitForSeconds(SpeedRate);
        }
    }

    static IEnumerator GameLoop()
    {
        //Start friction
        Instance.StartCoroutine(Friction());

        //Start acceleration
        Instance.StartCoroutine(Accelerate());

        while (true)
        {
            if(player.HasContactedWall())
                time += Time.deltaTime;

            if (time >= deadTime &&
                player.isActiveAndEnabled &&
                player.HasContactedWall() &&
                dontDestroy == false)
                DestoryPlayer();

            yield return null;
        }
    }

    static void DestoryPlayer()
    {

        playerDeathObj = ObjectPooler.GetMember("PlayerDeath", out ParticleSystem deathParticle);
        if(playerDeathObj != null && !playerDeathObj.activeInHierarchy)
        {
            playerDeathObj.SetActive(true);
            playerDeathObj.transform.localPosition = player.transform.localPosition;
            playerDeathObj.transform.rotation = Quaternion.identity;

            deathParticle.Play();
        }
        ResetTime();
        player.gameObject.SetActive(false);
    }

    
}
