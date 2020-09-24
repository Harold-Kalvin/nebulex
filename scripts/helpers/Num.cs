using System;

public static class Num
{
    /// <summary>
    /// Clamping a value to be sure it lies between two values
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
}
