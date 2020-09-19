using Godot;
using System;

public class BlackHole : KinematicBody2D
{
    public override void _Ready()
    {
        AddToGroup("bodies");
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), 30, new Color("#000000"));
    }
}
