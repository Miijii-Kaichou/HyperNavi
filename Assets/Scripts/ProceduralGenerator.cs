using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Event = EventManager.Event;

public enum Side
{
    LEFT,
    RIGHT,
    TOP,
    BOTTOM
}
public class ProceduralGenerator : MonoBehaviour
{
    private static ProceduralGenerator Instance;

    //All Environmental Layers
    public OpeningPath[] environmentalLayoutPaths;

    public ObjectPooler objectPooler;

    public OpeningPath currentLayout;
    public static OpeningPath CurrentPath;

    public OpeningPath previousLayout;
    public static OpeningPath PreviousPath;

    public static bool DontDeactivate = true;

    static List<OpeningPath> matchingPaths = new List<OpeningPath>();
    static OpeningPath path;

    public static float generationCoolDownTime = 0;

    // Toggling this on will only use Layout's 0 and 1 in a loop
    // This effect is used for the title screen
    public static bool OnPreview { get; set; } = true;
    public static bool IsStalling { get; private set; }
    public static float Time { get; private set; }
    public static bool IsGenerating { get; private set; }
    public static Event GenerateEvent;

    private void Awake()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        GenerateEvent = EventManager.AddNewEvent(EventManager.FreeValue, "callGenerate");
        GetAllEnvironmentalLayouts();
        StartCoroutine(CoolDownCycle());
    }

    /// <summary>
    /// Remove all active paths
    /// </summary>
    public static void StripPaths()
    {
        int size = Instance.environmentalLayoutPaths.Length;
        for (int iter = 0; iter < size; iter++)
        {
            OpeningPath path = Instance.environmentalLayoutPaths[iter];

            if (path != CurrentPath && path != PreviousPath)
            {
                path.transform.position = ObjectPooler.Position();
                path.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Generation Cool Down Cycle
    /// </summary>
    /// <returns></returns>
    IEnumerator CoolDownCycle()
    {
        while (true)
        {
            if (IsGenerating)
            {
                generationCoolDownTime += UnityEngine.Time.deltaTime;

                if (generationCoolDownTime >= 0.5f)
                {
                    generationCoolDownTime = 0;
                    IsGenerating = false;
                }
            }

            yield return null;
            ;
        }
    }

    /// <summary>
    /// Get's All Layouts. Has to be child of this object to work.
    /// </summary>
    void GetAllEnvironmentalLayouts()
    {
        OpeningPath[] foundChildren = ObjectPooler.pooledObjects.GetAllPaths();

        //Check if under Environmental Tag
        foreach (OpeningPath child in foundChildren)
        {
            //If even 1 child doesn't match the tag, get out of the function
            if (child != null && child.gameObject.layer != 1 << 9)
                break;
        }

        environmentalLayoutPaths = foundChildren;
    }

    //Generate a layout and place it some many units relative to the active layout
    public static void GenerateLayout(Side side, OpeningPath relativePath, OpeningPath existingPath)
    {
        float horSign = 0;
        float verSign = 0;

        if (!OnPreview && !IsGenerating)
        {
            IsGenerating = true;

            switch (side)
            {
                //I need a layout with left side opened
                case Side.LEFT:
                    existingPath = GetOpeningPath(side);
                    horSign = 1;
                    break;

                //I need a layout with right side opened
                case Side.RIGHT:
                    existingPath = GetOpeningPath(side);
                    horSign = -1;
                    break;

                //I need a layout with top side opened
                case Side.TOP:
                    existingPath = GetOpeningPath(side);
                    verSign = -1;
                    break;

                //I need a layout with bottom side opened
                case Side.BOTTOM:
                    existingPath = GetOpeningPath(side);
                    verSign = 1;
                    break;

                default:
                    break;
            }
        }

        else
        {
            //Need bottom side open
            side = Side.BOTTOM;
            existingPath = GetOpeningPath(side);
            verSign = 1;
        }

        //Now we spawn this path so many units from the trigger poinnt
        if (existingPath != null && !existingPath.gameObject.activeInHierarchy)
        {
            existingPath.gameObject.SetActive(true);
            existingPath.transform.localPosition = relativePath.transform.localPosition + new Vector3(horSign * existingPath.GetXUnit(), verSign * existingPath.GetYUnit(), 0f);
            existingPath.transform.rotation = Quaternion.identity;
        }
    }


    /// <summary>
    /// Iterate through the child layouts, and get an opening based on parameters
    /// If more than one with the amount of openings, a random layout will be chosen
    /// </summary>
    /// <returns></returns>
    static OpeningPath GetOpeningPath(Side side)
    {
        if (Instance != null && matchingPaths != null)
        {
            for (int iter = 0; iter < Instance.environmentalLayoutPaths.Length; iter++)
            {
                path = Instance.environmentalLayoutPaths[iter];

                if (path != null)
                {
                    switch (side)
                    {
                        case Side.LEFT:
                            if (path.IsLeftOpen())
                                matchingPaths.Add(path);
                            break;

                        case Side.RIGHT:
                            if (path.IsRightOpen())
                                matchingPaths.Add(path);
                            break;

                        case Side.TOP:
                            if (path.IsTopOpen())
                                matchingPaths.Add(path);
                            break;

                        case Side.BOTTOM:
                            if (path.IsBottomOpen())
                                matchingPaths.Add(path);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        // Now, return a random matching path
        int value = Random.Range(0, matchingPaths.Count - 1);

        if (!OnPreview && ObjectPooler.Exists())
            ObjectPooler.GetMember(matchingPaths[value].name.Replace("(Clone)", string.Empty), out path);
        else
            ObjectPooler.GetMember("Layout000Grid", out path);

        matchingPaths.Clear();

        return path;
    }
    public static OpeningPath[] GetAllPaths() => Instance.environmentalLayoutPaths;

    internal static void Stall(float v)
    {
        Instance.StartCoroutine(StallCycle(v));
    }

    static IEnumerator StallCycle(float v)
    {
        IsStalling = true;
        while (true)
        {
            if (IsStalling)
            {
                Time += UnityEngine.Time.deltaTime;

                if (Time >= v)
                {
                    IsStalling = false;
                    Time = 0;
                    break;
                }
            }

            yield return null;
        }
    }

    public static bool Exists() => Instance != null;
}

