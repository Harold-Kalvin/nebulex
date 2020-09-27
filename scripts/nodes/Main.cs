using Godot;
using System;

public class Main : Node2D
{   
    private ShootingStars _shootingStars;
    private ClickDetector _clickDetector;
    private SwipeDetector _swipeDetector;

    public override void _Ready()
    {
        _shootingStars = (ShootingStars)GetNode("ShootingStars");
        _clickDetector = (ClickDetector)GetNode("ClickDetector");
        _swipeDetector = (SwipeDetector)GetNode("SwipeDetector");
        _clickDetector.Connect("Clicked", this, nameof(_OnClick));
        _swipeDetector.Connect("Swiped", this, nameof(_OnSwipe));
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

    private void _OnClick()
    {
        _shootingStars.Sprint();
    }

    private void _OnSwipe(SwipeDirection direction)
    {
        if (direction == SwipeDirection.Left)
        {
            _shootingStars.MoveLeft();
        }
        else if (direction == SwipeDirection.Right)
        {
            _shootingStars.MoveRight();
        }
    }
}
