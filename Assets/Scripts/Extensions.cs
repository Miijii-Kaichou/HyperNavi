using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All defined extensions are placed here
/// </summary>
public static class Extensions
{
    public static OpeningPath[] GetPathsFromPrefab(this GameObject[] prefabs)
    {
        List<OpeningPath> paths = new List<OpeningPath>();
        foreach (GameObject prefab in prefabs)
        {
            paths.Add(prefab.GetComponent<OpeningPath>());
        }

        return paths.ToArray();
    }
}
