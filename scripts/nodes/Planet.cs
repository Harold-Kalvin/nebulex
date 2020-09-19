using Godot;
using System;

public class Planet : Polygon2D
{
    public float Radius;

    public override void _Ready()
    {
        AddToGroup("bodies");
        
        // setting radius from screen size
        GD.Randomize();
        var screenSize = GetViewport().GetVisibleRect().Size.x;
        Radius = (float)GD.RandRange((screenSize * 0.1F) / 2, (screenSize * 0.35F) / 2);
        var collisionShape = (CircleShape2D)GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
        collisionShape.Radius = Radius;
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), Radius, new Color("#6A0DAD"));
    }
}
