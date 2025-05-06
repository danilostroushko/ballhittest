using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListExtensions
{
    public static T Random<T>(this IEnumerable<T> enumerable)
    {
        var r = new System.Random();
        var list = enumerable as IList<T> ?? enumerable.ToList();
        return list.ElementAt(r.Next(0, list.Count()));
    }

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        System.Random rnd = new System.Random();
        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
    
    public static List<T> ShiftLeft<T>(this List<T> list, int shiftBy)
    {
        if (list.Count <= shiftBy)
        {
            return list;
        }

        var result = list.GetRange(shiftBy, list.Count - shiftBy);
        result.AddRange(list.GetRange(0, shiftBy));
        return result;
    }

    public static List<T> ShiftRight<T>(this List<T> list, int shiftBy)
    {
        if (list.Count <= shiftBy)
        {
            return list;
        }

        var result = list.GetRange(list.Count - shiftBy, shiftBy);
        result.AddRange(list.GetRange(0, list.Count - shiftBy));
        return result;
    }
}