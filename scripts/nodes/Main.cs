using Godot;
using System;

public class Main : Node2D
{   
    private ShootingStars _shootingStars; 
    private PackedScene _blackHoleScene = GD.Load<PackedScene>("res://scenes/BlackHole.tscn");

    public override void _Ready()
    {
        _shootingStars = (ShootingStars)GetNode("ShootingStars");
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed)
            {
                var globalMousePos = GetGlobalMousePosition();
                AddBlackHole(globalMousePos);
                _shootingStars.SetFollowPoint(globalMousePos);
            }
        }
    }

    public override void _Process(float delta)
    {
        // remove bodies that are not visible anymore
        foreach (Node2D body in GetTree().GetNodesInGroup("bodies"))
        {
            var viewportHeight = GetViewport().GetVisibleRect().Size.y;
            var bodyPosition = body.GetGlobalTransformWithCanvas()[2].y;
            if (bodyPosition > viewportHeight * 2)
            {
                body.QueueFree();
            }
        }
    }

    public void AddBlackHole(Vector2 pos)
    {
        // create the new black hole
        var blackHole = (BlackHole)_blackHoleScene.Instance();
        blackHole.Position = pos;
        AddChild(blackHole);
    }
}
