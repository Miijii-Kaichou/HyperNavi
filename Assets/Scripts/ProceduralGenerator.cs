using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    //All Environmental Layers
    public GameObject[] environmentalLayoutPrefabs;

    const string ENVIRONMENT_LAYOUT_TAG = "EnvironmentLayout";
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
}
