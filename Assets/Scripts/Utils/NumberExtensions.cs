using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberExtensions
{
    public static bool InRange<T>(this T value, T from, T to) where T : IComparable<T>
    {
        return value.CompareTo(from) >= 0 && value.CompareTo(to) <= 0;
    }

    public static float RangeToPercent(this float value, float min, float max)
    {
        value = Mathf.Clamp(value, min, max);
        return (value - min) / (max - min);
    }

    public static float RangeToPercent(this double value, double min, double max)
    {
        value = Clamp(value, min, max);
        return (float)((value - min) / (max - min));
    }

    public static float RangeToPercent(this int value, int min, int max)
    {
        value = Clamp(value, min, max);
        return (float)(value - min) / (float)(max - min);
    }

    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        else if (val.CompareTo(max) > 0) return max;
        else return val;
    }

    public static float PercentToRange(this float value, float min, float max)
    {
        return (value * (max - min)) + min;
    }

    public static double PercentToRange(this double value, double min, double max)
    {
        return (value * (max - min)) + min;
    }

    public static int PercentToRange(this float value, int min, int max)
    {
        return Mathf.CeilToInt((value * (max - min)) + min);
    }

    public static int PercentToCount(this float percent, int total)
    {
        return Mathf.CeilToInt(total * percent);
    }
}