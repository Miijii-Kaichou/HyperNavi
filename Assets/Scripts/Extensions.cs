using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// All defined extensions are placed here
/// </summary>
public static class Extensions
{
    public static OpeningPath[] GetAllPaths(this GameObject[] objectList)
    {
        List<OpeningPath> paths = new List<OpeningPath>();
        OpeningPath path;

        foreach (GameObject obj in objectList)
        {
            path = obj.GetComponent<OpeningPath>();
            if (path != null)
                paths.Add(path);
        }

        return paths.ToArray();
    }
    public static bool ContainsComponent<T>(this Collider2D collision) where T : Component
    {
        return (collision.GetComponent<PlayerPawn>() != null);
    }

    public static LoadSceneMode DetermineMode(this string value)
    {
        return value == "S".ToLower() ? LoadSceneMode.Single : LoadSceneMode.Additive;
    }

    /// <summary>
    /// Convert an object to a boolean
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool ToBoolean(this object obj)
    {
        try
        {
            if (obj.GetType() == typeof(int))
                return (int)obj == 1;

            if (obj.GetType() == typeof(string))
                return ((string)obj == "true".ToLower() || (string)obj == "1");

            if (obj.GetType() == typeof(char))
                return ((string)obj == "t".ToLower() || (char)obj == '1');

            throw new FormatException("Failed to convert to boolean.");
        }
        catch (FormatException fe)
        {
            Debug.LogException(fe);
            throw fe;
        }
    }

    public static bool NotUsed(this GameObject obj)
    {
        return (obj != null && !obj.activeInHierarchy);
    }

    public static void Reset(this object obj)
    {
            if(obj.GetType() == typeof(int) ||
            obj.GetType() == typeof(float) ||
            obj.GetType() == typeof(double))
                obj = 0;
    }

    public static bool Active(this GameObject obj)
    {
        return (obj != null && obj.activeInHierarchy);
    }

    public static void ClearString(this string _)
    {
        _ = string.Empty;
    }
}
