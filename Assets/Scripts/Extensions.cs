using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// All defined extensions are placed here
/// </summary>
public static class Extensions
{
    public static OpeningPath[] GetAllPaths(this List<GameObject> objectList)
    {
        List<OpeningPath> paths = new List<OpeningPath>();
        foreach(GameObject obj in objectList)
        {
            paths.Add(obj.GetComponent<OpeningPath>());
        }

        return paths.ToArray();
    }
    public static bool ContainsComponent<T>(this Collider2D collision) where T : Component
    {
        return (collision.GetComponent<PlayerPawn>() != null);
    }
}
