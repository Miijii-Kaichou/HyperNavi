using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// All defined extensions are placed here
/// </summary>
public static class Extensions
{
    public static OpeningPath[] GetPathsFromPrefab(this GameObject[] prefabs)
    {
        try
        {
            List<OpeningPath> paths = new List<OpeningPath>();
            foreach (GameObject prefab in prefabs)
            {
                paths.Add(prefab.GetComponent<OpeningPath>());
            }

            return paths.ToArray();
        } catch (Exception e)
        {
            throw e;
        }
    }
}
