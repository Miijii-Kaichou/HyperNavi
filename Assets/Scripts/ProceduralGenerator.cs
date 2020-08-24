using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Extensions;
using Random = UnityEngine.Random;

public enum Side
{
    LEFT,
    RIGHT,
    TOP,
    BOTTOM
}
public class ProceduralGenerator : MonoBehaviour
{
    //All Environmental Layers
    public GameObject[] environmentalLayoutPrefabs;

    public ObjectPooler objectPooler;

    public OpeningPath currentLayout;

    public OpeningPath previousLayout;

    public bool dontDeactivate = true;

    const string ENVIRONMENT_LAYOUT_TAG = "EnvironmentLayout";

    // Toggling this on will only use Layout's 0 and 1 in a loop
    // This effect is used for the title screen
    [SerializeField]
    private bool onPreview;
    // Start is called before the first frame update
    void Start()
    {
        GetAllEnvironmentalLayouts();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Get's All Layouts. Has to be child of this object to work.
    /// </summary>
    void GetAllEnvironmentalLayouts()
    {
        GameObject[] foundChildren = objectPooler.pooledObjects.ToArray();

        //Check if under Environmental Tag
        foreach (GameObject child in foundChildren)
        {
            //If even 1 child doesn't match the tag, get out of the function
            if (!child.CompareTag(ENVIRONMENT_LAYOUT_TAG))
            {
                Debug.Log("No Environment Tag...");
            }
        }

        environmentalLayoutPrefabs = foundChildren;
    }

    //Generate a layout and place it some many units relative to the active layout
    public void GenerateLayout(Side side, OpeningPath relativePath)
    {
        OpeningPath path = null;
        float horSign = 0;
        float verSign = 0;
        while (path == null)
        {
            switch (side)
            {
                case Side.LEFT:
                    path = GetOpeningPath(side);
                    horSign = 1;
                    break;
                case Side.RIGHT:
                    path = GetOpeningPath(side);
                    horSign = -1;
                    break;
                case Side.TOP:
                    path = GetOpeningPath(side);
                    verSign = -1;
                    break;
                case Side.BOTTOM:
                    path = GetOpeningPath(side);
                    verSign = 1;
                    break;
                default:
                    break;
            }
        }

        //Now we spawn this path so many units from the trigger poinnt
        if (path != null && !path.gameObject.activeInHierarchy)
        {
            path.gameObject.SetActive(true);
            path.transform.localPosition = relativePath.transform.localPosition + new Vector3(horSign * path.GetXUnit(), verSign * path.GetYUnit(), 1f);
            path.transform.rotation = Quaternion.identity;
        } else
        {
            Debug.Log("If you got this, apparently path is null...");
        }
    }


    /// <summary>
    /// Iterate through the child layouts, and get an opening based on parameters
    /// If more than one with the amount of openings, a random layout will be chosen
    /// </summary>
    /// <returns></returns>
    OpeningPath GetOpeningPath(Side side)
    {
        OpeningPath[] paths = environmentalLayoutPrefabs.GetPathsFromPrefab();
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
        int value = Random.Range(0, matchingPaths.Count-1);
        return matchingPaths[value];
    }
}
