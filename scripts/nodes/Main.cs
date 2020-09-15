using Godot;
using System;

public class Main : Node2D
{   
    public PackedScene blackHoleScene = GD.Load<PackedScene>("res://scenes/BlackHole.tscn");
    
    private Vector2 _screenSize;
    private ShootingStar _shootingStar;

    public override void _Ready()
    {
        _screenSize = GetViewport().GetVisibleRect().Size;

        // set shooting star position
        _shootingStar = (ShootingStar)GetNode("ShootingStar");
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed)
            {   
                var camera = (Camera2D)_shootingStar.GetNode("Camera");
                var defaultPos = new Vector2(_screenSize.x / 2, (_screenSize.y / 2) + 250);
                var realPosition = (touchEvent.Position - defaultPos) + camera.GlobalPosition;

                AddBlackHole(realPosition);
                _shootingStar.PositionToFollow = realPosition;
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
