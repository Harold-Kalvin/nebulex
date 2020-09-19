using Godot;
using System;

public class BlackHole : Polygon2D
{
    public float Radius;

    public override void _Ready()
    {
        AddToGroup("bodies");
        
        // setting radius from screen size
        Radius = (GetViewport().GetVisibleRect().Size.x * 0.1F) / 2;
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), Radius, new Color("#000000"));
    }
}
