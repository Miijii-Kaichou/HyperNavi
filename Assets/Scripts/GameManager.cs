using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

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

    [SerializeField]
    private OpeningPath startingLayout;

    [SerializeField]
    private UnityEvent @onPlayerDeath = new UnityEvent();

    static AsyncOperation operation = new AsyncOperation();

    // Handles all gaming things in the game

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
    private static float deadTime = 0.05f;

    private const int DEFAULT_FRAMERATE = 60;

    private static GameObject playerDeathObj;

    public static float loadingProgress = 0f;

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

    /// <summary>
    /// Determine the timing upon taking a turn
    /// </summary>
    /// <param name="distance"></param>
    public static void DetermineTiming(ref float distance)
    {
        dontDestroy = (distance < 1f);
    }

    /// <summary>
    /// Allow the player to get destoryed (deactivated)
    /// </summary>
    public static void AllowDestructionOfPlayer() => dontDestroy = false;

    /// <summary>
    /// Reset time
    /// </summary>
    public static void ResetTime()
    {
        time = RESET;
    }

    /// <summary>
    /// Apply friction
    /// </summary>
    /// <returns></returns>
    static IEnumerator Friction()
    {
        while (true)
        {
            BoostSpeed = Mathf.Lerp(BoostSpeed, 0f, BoostDepletionValue);
            yield return new WaitForSeconds(BoostSlowdownRate);
        }
    }

    /// <summary>
    /// Acceleration
    /// </summary>
    /// <returns></returns>
    static IEnumerator Accelerate()
    {
        while (true)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, SpeedAcceleration);
            yield return new WaitForSeconds(SpeedRate);
        }
    }

    /// <summary>
    /// Official Gameloop
    /// </summary>
    /// <returns></returns>
    static IEnumerator GameLoop()
    {
        //Start friction
        Instance.StartCoroutine(Friction());

        //Start acceleration
        Instance.StartCoroutine(Accelerate());

        while (true)
        {
            if (player.HasContactedWall())
                time += Time.deltaTime;

            if (time >= deadTime)
            {
                AllowDestructionOfPlayer();
                if (
                player.isActiveAndEnabled &&
                player.HasContactedWall() &&
                dontDestroy == false)
                    DestoryPlayer();
            }
            yield return null;
        }
    }

    static void DestoryPlayer()
    {
        //Get player death effect object
        playerDeathObj = ObjectPooler.GetMember("PlayerDeath", out ParticleSystem deathParticle);

        if (playerDeathObj != null && !playerDeathObj.activeInHierarchy)
        {
            playerDeathObj.SetActive(true);
            playerDeathObj.transform.localPosition = player.transform.localPosition;
            playerDeathObj.transform.rotation = Quaternion.identity;

            //Play death particle animation
            deathParticle.Play();
        }

        //Rest time
        ResetTime();

        //Deactivate player
        player.gameObject.SetActive(false);

        //Trigger event
        Instance.onPlayerDeath.Invoke();
    }

    public static bool Exists() => Instance != null;

    /// <summary>
    /// Load scene
    /// Enter in event as: Name of Scene / 0 or 1 / S or A
    /// </summary>
    /// <param name="index"></param>
    public void LoadScene(string pattern)
    {
        //Tokenize string
        string[] values = pattern.Split('/');

        //Get scene name argument
        string sceneName = values[0];

        //Get async argument
        bool async = values[1].ToBoolean();

        //Get load scene mode argument
        LoadSceneMode mode = values[2].DetermineMode();

        //Call load scene
        LoadScene(sceneName, async, mode);
    }

    static void LoadScene(string sceneName, bool asynchronously = false, LoadSceneMode mode = LoadSceneMode.Single)
    {
        switch (asynchronously)
        {
            case true:
                Instance.StartCoroutine(AsyncronousLoad(sceneName, mode));
                break;
            case false:
                SceneManager.LoadScene(sceneName, mode);
                break;
        }
    }

    public static void UnloadScene(string sceneName, EventManager.Event @event)
    {
        Instance.StartCoroutine(AsynchronousUnload(sceneName, @event));
    }

    static IEnumerator AsyncronousLoad(string sceneName, LoadSceneMode mode)
    {
        operation = new AsyncOperation();
        operation = SceneManager.LoadSceneAsync(sceneName, mode);
        operation.allowSceneActivation = false;

        loadingProgress = Mathf.Clamp01(operation.progress / .9f);

        if (loadingProgress >= 0.99f)
            operation.allowSceneActivation = true;

        yield return operation;
    }

    static IEnumerator AsynchronousUnload(string sceneName, EventManager.Event @event)
    {
        operation = new AsyncOperation();
        operation = SceneManager.UnloadSceneAsync(sceneName);

        loadingProgress = Mathf.Clamp01(operation.progress / .9f);

        EventManager.TriggerEvent(@event.eventCode);

        yield return operation;
    }

    static void FlushPaths()
    {
        foreach (OpeningPath path in ObjectPooler.pooledObjects.GetAllPaths())
        {
            if (path.gameObject.activeInHierarchy)
                path.gameObject.SetActive(false);
        }
    }

    public static void SpawnPlayerToStartLayout()
    {
        if (!player.gameObject.activeInHierarchy)
        {
            CurrentSpeed = InitialSpeed;

            ResetTime();

            FlushPaths();

            OpeningPath layout = Instance.startingLayout;

            ProceduralGenerator.DontDeactivate();
            ProceduralGenerator.CurrentLayout = layout;

            layout.gameObject.SetActive(true);



            player.gameObject.SetActive(true);
            player.transform.localPosition = layout.GetSignalTriggerPosition();
            player.transform.rotation = Quaternion.identity;
            player.ChangeDirection(Direction.UP);
        }
    }

    public static void SpawnPlayerToLastSignalPoint()
    {
        if (!player.gameObject.activeInHierarchy)
        {
            CurrentSpeed = InitialSpeed;

            dontDestroy = true;

            ProceduralGenerator.DontDeactivate();

            //Assure that when reactivating player, they are facing towards any random open path
            bool leftOpened, rightOpened, topOpened, bottomOpened;

            FlushPaths();

            OpeningPath path = player.GetLastSignalPoint().GetPath();

            //Set path open
            path.gameObject.SetActive(true);

            //Check if one of side are open
            leftOpened = path.IsLeftOpen();
            rightOpened = path.IsRightOpen();
            topOpened = path.IsTopOpen();
            bottomOpened = path.IsBottomOpen();


            //Set player active
            player.gameObject.SetActive(true);

            //Spawn to last checkpoint
            player.transform.localPosition = path.GetSignal().transform.position;

            ResetTime();
            int value;
            bool pathFound = false;
            int safety = 0;
            while (pathFound == false)
            {
                value = Random.Range(0, 3);
                switch ((Direction)value)
                {
                    case Direction.LEFT:
                        if (leftOpened)
                        {
                            player.MoveLeft();
                            pathFound = true;
                        }
                        break;
                    case Direction.RIGHT:
                        if (rightOpened)
                        {
                            player.MoveRight();
                            pathFound = true;
                        }
                        break;
                    case Direction.UP:
                        if (topOpened)
                        {
                            player.MoveUp();
                            pathFound = true;
                        }
                        break;
                    case Direction.DOWN:
                        if (bottomOpened)
                        {
                            player.MoveDown();
                            pathFound = true;
                        }
                        break;
                }

                safety++;
                if (safety >= 100)
                {
                    Debug.Log("Iteration Failed");
                    break;
                }

            }
            
        }
    }
}

