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

    private static Vector2 _size = new Vector2();
    private static Vector2 _position = new Vector2();
    private Vector2 _viewportSize;

    public override void _Ready()
    {
        _viewportSize = GetViewport().GetVisibleRect().Size;
        var viewportRatioX = MIN_RATIO_X;
        var viewportRatioY = (_viewportSize.y / _viewportSize.x) * viewportRatioX;

        // set size
        _size.x = _viewportSize.x;
        _size.y = _viewportSize.y;

        // if viewport is wider than minimum aspect ratio
        if (viewportRatioY < MIN_RATIO_Y) {
            // force the minimum aspect ratio
            _size.x = MIN_RATIO_X * _size.y / MIN_RATIO_Y;
            // center
            _position.x = (_viewportSize.x - _size.x) / 2;
        }
    }
}