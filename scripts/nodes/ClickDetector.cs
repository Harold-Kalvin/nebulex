using Godot;
using System;

public class ClickDetector : Node2D
{
    [Signal]
    public delegate void Clicked();

    private Vector2 _clickStartPosition;
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
                _clickStartPosition = touchEvent.Position;
                _timer.Start();
            }
            else if (!_timer.IsStopped() && touchEvent.Position == _clickStartPosition)
            {
                _timer.Stop();
                EmitSignal("Clicked");
            }
        }
    }
}
