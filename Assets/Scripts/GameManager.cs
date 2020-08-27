using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public enum Result
{
    SUCCESS = 1,
    FAILURE = 0
}

public class GameManager : MonoBehaviour
{

    private static GameManager Instance;

    [SerializeField]
    private OpeningPath startingLayout;

    [SerializeField]
    private UnityEvent @onPlayerDeath = new UnityEvent();

    static AsyncOperation operation = new AsyncOperation();

    static ScoreSystem scoreSystem;

    // Handles all gaming things in the game

    /// <summary>
    /// The starting speed of the player
    /// </summary>
    public static float InitialSpeed { get; private set; } = 1f;

    /// <summary>
    /// The rate in which to increase speed
    /// </summary>
    public static float SpeedRate { get; private set; } = 0.5f;

    /// <summary>
    /// The speed added to current speed over a certain rate
    /// </summary>
    public static float SpeedAcceleration { get; private set; } = 0.01f;

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
    /// Current In-Game Currency
    /// </summary>
    public static int CurrentCurrency { get; private set; }

    /// <summary>
    /// The max speed of the player
    /// </summary>
    public const float MaxSpeed = 15f;

    /// <summary>
    /// A full second
    /// </summary>
    public const float FULL_SEC = 1;

    public const int RESET = 0;

    public static PlayerPawn player;

    public static bool dontDestroy = false;

    private static float time;
    private static float deadTime = 0.075f;

    private const int DEFAULT_FRAMERATE = 60;

    private static GameObject playerDeathObj;

    public static float loadingProgress = 0f;

    //bigger than one of the values
    public static float[] timingWindows =
    {
        0.35f,
        0.5f,
        0.6f,
        1
    };


    public static int[] points = {
        100,
        10,
        1,
        0
    };

    public static string[] comments =
    {
        "Excellent Turn",
        "Okay Turn",
        "Late Turn",
        "Bad Turn"
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);

            //Load Title Screen
            LoadScene("TitleScreen/1/A");

            Application.targetFrameRate = DEFAULT_FRAMERATE;

            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Start the game officially
    /// </summary>
    public static void StartGame()
    {
        IsGameStarted = true;

        ScoreSystem.Init();
        CurrencySystem.Init();

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
    public static void DetermineTiming(float distance)
    {
        dontDestroy = (distance < 1.5f);

        for (int iter = 0; iter < timingWindows.Length; iter++)
        {
            float timing = timingWindows[iter];
            if (distance <= timing)
            {
                ScoreSystem.AddToScore(points[iter]);
                return;
            }
        }
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

    /// <summary>
    /// Destory the player
    /// </summary>
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

        ScoreSystem.Stop();
        CurrencySystem.Stop();

        //Rest time
        ResetTime();

        //Deactivate player
        player.gameObject.SetActive(false);

        //Trigger event
        Instance.onPlayerDeath.Invoke();

        IsGameStarted = false;
    }

    /// <summary>
    /// Check if Game Manager Exists and in memory
    /// </summary>
    /// <returns></returns>
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

    
    /// <summary>
    /// Load a scene
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="asynchronously"></param>
    /// <param name="mode"></param>
    static void LoadScene(string sceneName, bool asynchronously = false, LoadSceneMode mode = LoadSceneMode.Single)
    {
        switch (asynchronously)
        {
            case true:
                Instance.StartCoroutine(AsynchronousLoad(sceneName, mode));
                break;
            case false:
                SceneManager.LoadScene(sceneName, mode);
                break;
        }
    }

    /// <summary>
    /// Unload a scene
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="event"></param>
    public static void UnloadScene(string sceneName, EventManager.Event @event)
    {
        Instance.StartCoroutine(AsynchronousUnload(sceneName, @event));
    }

    /// <summary>
    /// Unload a scene
    /// </summary>
    /// <param name="sceneName"></param>
    public static void UnloadScene(string sceneName)
    {
        Instance.StartCoroutine(AsynchronousUnload(sceneName));
    }

    /// <summary>
    /// Asynchronously Load a Scene
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    static IEnumerator AsynchronousLoad(string sceneName, LoadSceneMode mode)
    {
        operation = new AsyncOperation();
        operation = SceneManager.LoadSceneAsync(sceneName, mode);
        operation.allowSceneActivation = false;
        while (true)
        {
            loadingProgress = Mathf.Clamp01(operation.progress / .9f);

            if (loadingProgress >= .99f)
            {
                operation.allowSceneActivation = true;
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Asynchronously Load a Scene
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="event"></param>
    /// <returns></returns>
    static IEnumerator AsynchronousUnload(string sceneName, EventManager.Event @event)
    {
        operation = new AsyncOperation();
        operation = SceneManager.UnloadSceneAsync(sceneName);

        while (true)
        {

            loadingProgress = Mathf.Clamp01(operation.progress / .9f);

            if (loadingProgress >= .99f)
            {
                try
                {
                    @event.Trigger();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Asynchronously Unload a Scene
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    static IEnumerator AsynchronousUnload(string sceneName)
    {
        operation = new AsyncOperation();
        operation = SceneManager.UnloadSceneAsync(sceneName);

        while (true)
        {

            loadingProgress = Mathf.Clamp01(operation.progress / .9f);

            if (loadingProgress >= .99f)
                break;

            yield return null;
        }
    }

    /// <summary>
    /// Remove all active paths
    /// </summary>
    static void FlushPaths()
    {
        OpeningPath[] paths = ProceduralGenerator.GetAllPaths();
        int size = paths.Length;
        for(int iter = 0; iter < size; iter++ )
        {
            OpeningPath path = paths[iter];

            if (path.gameObject.activeInHierarchy)
                path.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Spawn the player to the starting layout
    /// </summary>
    public static void SpawnPlayerToStartLayout()
    {
        if (!player.gameObject.activeInHierarchy)
        {
            IsGameStarted = true;
            ScoreSystem.Resume();
            CurrencySystem.Resume();
            ScoreSystem.ResetScore();

            FlushPaths();

            CurrentSpeed = InitialSpeed;

            ResetTime();

            OpeningPath layout = Instance.startingLayout;

            ProceduralGenerator.CurrentLayout = null;
            ProceduralGenerator.PreviousLayout = null;

            layout.gameObject.SetActive(true);

            player.gameObject.SetActive(true);
            player.transform.localPosition = layout.GetSignalTriggerPosition(false);
            player.transform.rotation = Quaternion.identity;
            player.ChangeDirection(Direction.UP);  
        }
    }

    /// <summary>
    /// Reference Score System
    /// </summary>
    /// <param name="system"></param>
    public static void SetScoreSystem(ScoreSystem system) => scoreSystem = system;

    /// <summary>
    /// Submit Official Score
    /// </summary>
    /// <param name="value"></param>
    public static void ScoreSubmit(int value) => CurrentScore = value;

    /// <summary>
    /// Submit Currency
    /// </summary>
    /// <param name="currency"></param>
    public static void CurrencySumbit(int value) => CurrentCurrency = value;


    /// <summary>
    /// Spawn the player at the nearest turning point
    /// </summary>
    public static void SpawnPlayerToLastSignalPoint()
    {
        if (!player.gameObject.activeInHierarchy)
        {
            IsGameStarted = true;
            ScoreSystem.Resume();
            CurrencySystem.Resume();
            FlushPaths();

            dontDestroy = true;

            //Assure that when reactivating player, they are facing towards any random open path
            bool leftOpened, rightOpened, topOpened, bottomOpened;

            OpeningPath path = player.GetLastSignalPoint().GetPath();

            ProceduralGenerator.CurrentLayout = null;
            ProceduralGenerator.PreviousLayout = null;

            //Set path open
            path.gameObject.SetActive(true);

            //Check if one of side are open
            leftOpened = path.IsLeftOpen();
            rightOpened = path.IsRightOpen();
            topOpened = path.IsTopOpen();
            bottomOpened = path.IsBottomOpen();

            ResetTime();

            player.EnableIFrame();

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
                            //Set player active
                            player.gameObject.SetActive(true);

                            //Spawn to last checkpoint
                            player.transform.localPosition = path.GetSignal().transform.position;
                            player.MoveLeft();
                            pathFound = true;
                        }
                        break;
                    case Direction.RIGHT:
                        if (rightOpened)
                        {
                            //Set player active
                            player.gameObject.SetActive(true);

                            //Spawn to last checkpoint
                            player.transform.localPosition = path.GetSignal().transform.position;
                            player.MoveRight();
                            pathFound = true;
                        }
                        break;
                    case Direction.UP:
                        if (topOpened)
                        {
                            //Set player active
                            player.gameObject.SetActive(true);

                            //Spawn to last checkpoint
                            player.transform.localPosition = path.GetSignal().transform.position;
                            player.MoveUp();
                            pathFound = true;
                        }
                        break;
                    case Direction.DOWN:
                        if (bottomOpened)
                        {
                            //Set player active
                            player.gameObject.SetActive(true);

                            //Spawn to last checkpoint
                            player.transform.localPosition = path.GetSignal().transform.position;
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

    public static void AssignPlayer(PlayerPawn newPlayer) => player = newPlayer;
}

