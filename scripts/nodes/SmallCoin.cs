using Godot;
using System;

public class SmallCoin : Polygon2D
{
    public float Radius {
        get => _radius;
    }

    private float _radius;

    public override void _Ready()
    {
        AddToGroup("bodies");

        // setting radius from screen size
        _radius = (GetViewport().GetVisibleRect().Size.x * 0.0225F) / 2;
        var collisionShape = (CircleShape2D)GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
        collisionShape.Radius = _radius;
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), _radius, new Color("#FFFFFF"));
    }
}
