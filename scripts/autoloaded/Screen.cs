using Godot;
using System;

public class Screen : Node {
    public const float MIN_RATIO_X = 9;
    public const float MIN_RATIO_Y = 16;

    public static Vector2 Size {
        get => _size;
    }
    public static Vector2 Position {
        get => _position;
    }
    public static Vector2 MinRatioSize {
        get => _minRatioSize;
    }
    private static Vector2 _size = new Vector2();
    private static Vector2 _position = new Vector2();
    private static Vector2 _minRatioSize = new Vector2();
    private Vector2 _viewportSize;

    public override void _Ready()
    {
        _viewportSize = GetViewport().GetVisibleRect().Size;
        var viewportRatioX = MIN_RATIO_X;
        var viewportRatioY = (_viewportSize.y / _viewportSize.x) * viewportRatioX;

        // set size
        _size = _viewportSize;
        _minRatioSize = _viewportSize;

        // if viewport is wider than minimum aspect ratio
        if (viewportRatioY < MIN_RATIO_Y) {
            // force the minimum aspect ratio
            _size.x = MIN_RATIO_X * _size.y / MIN_RATIO_Y;
            _minRatioSize.x = MIN_RATIO_X * _minRatioSize.y / MIN_RATIO_Y;
            // center
            _position.x = (_viewportSize.x - _size.x) / 2;
        }
        else {
            _minRatioSize.y = MIN_RATIO_Y * _minRatioSize.x / MIN_RATIO_X;
        }
    }
}