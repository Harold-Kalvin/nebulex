using Godot;
using System;

public class Main : Node2D
{   
    private ShootingStars _shootingStars;
    private SwipeDetector _swipeDetector;

    public override void _Ready()
    {
        _shootingStars = (ShootingStars)GetNode("ShootingStars");
        _swipeDetector = (SwipeDetector)GetNode("SwipeDetector");
        _swipeDetector.Connect("Swiped", this, nameof(_OnSwiped));
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed)
            {
                _shootingStars.Sprint();
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

    private void _OnSwiped(SwipeDirection direction)
    {
        GD.Print($"swiped {direction}");
    }
}
