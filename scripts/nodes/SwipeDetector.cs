using Godot;
using System;

public enum SwipeDirection
{
    Top,
    Right,
    Bottom,
    Left,
}

public class SwipeDetector : Node2D
{
    [Signal]
    public delegate void Swiped(SwipeDirection direction);

    private float _maxDiagonalSlope = 1.35f;
    private float _minSwipeLength = 20;
    private Vector2 _swipeStartPosition;
    private Timer _timer;

    public override void _Ready()
    {
        _timer = (Timer)GetNode("Timer");
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed)
            {
                _StartDetection(touchEvent.Position);
            }
            // on release
            else if (!_timer.IsStopped() && touchEvent.Position != _swipeStartPosition)
            {
                _EndDetection(touchEvent.Position);
            }
        }
    }

    private void _StartDetection(Vector2 position)
    {
        _swipeStartPosition = position;
        _timer.Start();
    }

    private void _EndDetection(Vector2 position)
    {
        _timer.Stop();
        var direction = (position - _swipeStartPosition).Normalized();
        
        // swipe must have a minimum length
        if (position.DistanceTo(_swipeStartPosition) <= _minSwipeLength)
        {
            return;
        }
        
        // diagonal swipe is invalid
        if (Mathf.Abs(direction.x) + Mathf.Abs(direction.y) >= _maxDiagonalSlope)
        {
            return;
        }

        // emit horizontal swipe
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x < 0)
            {
                EmitSignal("Swiped", SwipeDirection.Left);
            }
            else if (direction.x > 0)
            {
                EmitSignal("Swiped", SwipeDirection.Right);
            }
        }

        // emit vertical swipe
        else if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            if (direction.y < 0)
            {
                EmitSignal("Swiped", SwipeDirection.Top);
            }
            else if (direction.y > 0)
            {
                EmitSignal("Swiped", SwipeDirection.Bottom);
            }
        }
    }
}
