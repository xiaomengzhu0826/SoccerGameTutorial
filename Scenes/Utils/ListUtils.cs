using Godot;
using System;
using System.Collections.Generic;

public static class ListUtils 
{
    /// <summary>
    /// 对任意带位置的对象列表按与目标位置的距离进行排序（近 → 远）。
    /// </summary>
    public static void SortByDistance<T>(this List<T> list, Vector2 targetPosition, Func<T, Vector2> getPosition)
    {
        list.Sort((a, b) =>
        {
            float distA = getPosition(a).DistanceSquaredTo(targetPosition);
            float distB = getPosition(b).DistanceSquaredTo(targetPosition);
            return distA.CompareTo(distB);
        });
    }
}

