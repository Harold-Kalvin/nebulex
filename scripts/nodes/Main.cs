using Godot;
using System;

public class Main : Node2D
{   
    public PackedScene blackHoleScene = GD.Load<PackedScene>("res://scenes/BlackHole.tscn");
    
    private ShootingStar _shootingStar;

    public override void _Ready()
    {
        // set shooting star position
        _shootingStar = (ShootingStar)GetNode("ShootingStar");
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed)
            {
                var globalMousePos = GetGlobalMousePosition();
                AddBlackHole(globalMousePos);
                _shootingStar.PositionToFollow = globalMousePos;
            }
        }
    }

    public void AddBlackHole(Vector2 pos)
    {
        // free each existing black holes first
        foreach (BlackHole existing in GetTree().GetNodesInGroup("blackHole"))
        {
            existing.QueueFree();
        }
        
        // create the new black hole
        var blackHole = (BlackHole)blackHoleScene.Instance();
        blackHole.Position = pos;
        AddChild(blackHole);
    }
}
