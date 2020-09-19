using Godot;
using System;

public class ShootingStar : KinematicBody2D
{
    [Export]
    private float _maxSpeed = 60;
    [Export]
    private float _seekForce = 0.5F;

    public Vector2 PositionToFollow;

    private Vector2 _velocity;

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        if (PositionToFollow != new Vector2(0, 0))
        {
            var acceleration = _Seek(PositionToFollow);
            _velocity += acceleration * delta;
            _velocity.y = Num.Clamp(_velocity.y, -_maxSpeed, -_maxSpeed / 3); // keep moving forward
            var _collision = MoveAndCollide(_velocity);
        }
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), 15, new Color("#FFFFFF"));
    }

    private Vector2 _Seek(Vector2 target)
    {
        var desired = (target - Position).Normalized() * _maxSpeed;
        var steer = desired - _velocity;
        return steer * _seekForce;
    }
}
