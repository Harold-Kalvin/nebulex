using Godot;
using System;

public class Camera : Camera2D
{
    [Export]
    private float _offsetScreenX = 0.5F;
    [Export]
    private float _offsetScreenY = 0.75F;

    private Vector2 _screenSize;

    public override void _Ready()
    {
        _screenSize = GetViewport().GetVisibleRect().Size;
        // setting camera offset
        Offset = new Vector2(-(_screenSize.x * _offsetScreenX), -(_screenSize.y * _offsetScreenY));
    }
}
