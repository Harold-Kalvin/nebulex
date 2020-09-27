using System;

public static class Num
{
    /// <summary>
    /// Returns the clamped value that lies between the min and max values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        var _Result = value;
        if (value.CompareTo(max) > 0)
            _Result = max;
        else if (value.CompareTo(min) < 0)
            _Result = min;
        return _Result;
    }

    /// <summary>
    /// Returns true if the value is between the min and max values, false otherwise.
    /// </summary>
    /// <typeparam name="bool"></typeparam>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static bool Between(float value, float min, float max, bool inclusive=true)
    {
        if (min > max)
        {
            throw new System.ArgumentException("<min> must be lower or equal to <max>", "min");
        }
        return inclusive ? value >= min && value <= max : value > min && value < max;
    }
}
