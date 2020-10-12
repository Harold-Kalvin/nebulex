using Godot;
using System;

public class Coin : Polygon2D
{
    public float Radius {
        get => _radius;
    }

    private float _radius;
    private float _coinMinSizeScreenX = 0.12f;
    private float _coinMaxSizeScreenX = 0.18f;

    public override void _Ready()
    {
        AddToGroup("bodies");

        // setting radius from screen size
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        _radius = (float)GD.RandRange(
            (screenSize.x * _coinMinSizeScreenX) / 2,
            (screenSize.x * _coinMaxSizeScreenX) / 2
        );
        var collisionShape = (CircleShape2D)GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
        collisionShape.Radius = _radius;
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), _radius, new Color("#FFFFFF"));
    }
}