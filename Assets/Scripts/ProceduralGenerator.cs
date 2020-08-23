using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Extensions;
using Random = UnityEngine.Random;

public class ProceduralGenerator : MonoBehaviour
{
    //All Environmental Layers
    public GameObject[] environmentalLayoutPrefabs;

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
        GameObject[] foundChildren = transform.parent.GetComponentsInChildren<GameObject>();

        //Check if under Environmental Tag
        foreach(GameObject child in foundChildren)
        {
            //If even 1 child doesn't match the tag, get out of the function
            if (!child.CompareTag(ENVIRONMENT_LAYOUT_TAG)) return;        
        }

        environmentalLayoutPrefabs = foundChildren;
    }

    //Generate a layout and place it some many units relative to the active layout
    void GenerateLayout()
    {

    }

    
    /// <summary>
    /// Iterate through the child layouts, and get an opening based on parameters
    /// If more than one with the amount of openings, a random layout will be chosen
    /// </summary>
    /// <returns></returns>
    OpeningPath GetOpeningPath(bool leftOpened, bool rightOpened, bool topOpened, bool bottomOpened)
    {
        OpeningPath[] paths = environmentalLayoutPrefabs.GetPathsFromPrefab();
        List<OpeningPath> matchingPaths = new List<OpeningPath>();
        foreach(OpeningPath path in paths)
        {
            if(
                path.IsLeftOpen() == leftOpened ||
                path.IsRightOpen() == rightOpened ||
                path.IsTopOpen() == topOpened ||
                path.IsBottomOpen() == bottomOpened
                )
            {
                matchingPaths.Add(path);
            }
        }

        // Now, return a random matching path
        int value = Random.Range(0, matchingPaths.Count - 1);
        return matchingPaths[value];
    }
}
