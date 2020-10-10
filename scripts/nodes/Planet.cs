using Godot;
using System;

public enum PlanetType
{
    Big,
    Small,
}

public class Planet : Polygon2D
{
    public float Radius {
        get => _radius;
        set {
            _radius = value;
            // set collision shape from radius
            var collisionShape = (CircleShape2D)GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
            collisionShape.Radius = _radius;
        }
    }

    private float _radius;

    public override void _Ready()
    {
        AddToGroup("bodies");
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), _radius, new Color("#6A0DAD"));
    }
}
