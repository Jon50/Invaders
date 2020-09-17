using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool DoesNotContain<T>(this List<T> list, T item)
    {
        return !list.Contains(item);
    }

    public static bool NotTaggedAs(this Collider2D collider, string tag)
    {
        return !collider.CompareTag(tag);
    }

    public static bool IsNull(this object instance)
    {
        if (instance == null)
            return true;
        return false;
    }

    public static bool IsNotNull(this object instance)
    {
        if (instance != null)
            return true;
        return false;
    }

    public static float LinearToDecibel(this float linear)
    {
        return (linear != 0) ? 20f * Mathf.Log10(linear) : -144f;
    }
}