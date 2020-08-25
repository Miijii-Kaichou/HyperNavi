using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Threading;

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

    public OpeningPath previousLayout;

    public bool dontDeactivate = true;

    const string ENVIRONMENT_LAYOUT_TAG = "EnvironmentLayout";

    // Toggling this on will only use Layout's 0 and 1 in a loop
    // This effect is used for the title screen
    public static bool OnPreview { get; set; } = true;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        } else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetAllEnvironmentalLayouts();
    }

    /// <summary>
    /// Get's All Layouts. Has to be child of this object to work.
    /// </summary>
    void GetAllEnvironmentalLayouts()
    {
        OpeningPath[] foundChildren = objectPooler.pooledObjects.GetAllPaths();

        //Check if under Environmental Tag
        foreach (OpeningPath child in foundChildren)
        {
            //If even 1 child doesn't match the tag, get out of the function
            if (!child.CompareTag(ENVIRONMENT_LAYOUT_TAG))
            {
                Debug.Log("No Environment Tag...");
                break;
            }
        }

        environmentalLayoutPaths = foundChildren;
    }

    //Generate a layout and place it some many units relative to the active layout
    public void GenerateLayout(Side side, OpeningPath relativePath)
    {
        OpeningPath path = null;
        float horSign = 0;
        float verSign = 0;
        if (!OnPreview)
        {
            switch (side)
            {
                //I need a layout with left side opened
                case Side.LEFT:
                    path = GetOpeningPath(side);
                    horSign = 1;
                    break;

                //I need a layout with right side opened
                case Side.RIGHT:
                    path = GetOpeningPath(side);
                    horSign = -1;
                    break;

                //I need a layout with top side opened
                case Side.TOP:
                    path = GetOpeningPath(side);
                    verSign = -1;
                    break;

                //I need a layout with bottom side opened
                case Side.BOTTOM:
                    path = GetOpeningPath(side);
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
            path = GetOpeningPath(side);
            verSign = 1;
        }

        //Now we spawn this path so many units from the trigger poinnt
        if (path != null && !path.gameObject.activeInHierarchy)
        {
            path.gameObject.SetActive(true);
            path.transform.localPosition = relativePath.transform.localPosition + new Vector3(horSign * path.GetXUnit(), verSign * path.GetYUnit(), 0f);
            path.transform.rotation = Quaternion.identity;
        }
    }


    /// <summary>
    /// Iterate through the child layouts, and get an opening based on parameters
    /// If more than one with the amount of openings, a random layout will be chosen
    /// </summary>
    /// <returns></returns>
    OpeningPath GetOpeningPath(Side side)
    {
        OpeningPath[] paths = environmentalLayoutPaths;
        List<OpeningPath> matchingPaths = new List<OpeningPath>();

        foreach (OpeningPath path in paths)
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

        // Now, return a random matching path
        int value = Random.Range(0, matchingPaths.Count - 1);

        OpeningPath pooledPath;

        if (!OnPreview)
            ObjectPooler.GetMember(matchingPaths[value].name.Replace("(Clone)", string.Empty), out pooledPath);
        else
            ObjectPooler.GetMember("Layout000Grid", out pooledPath);

        return pooledPath;
    }
}

