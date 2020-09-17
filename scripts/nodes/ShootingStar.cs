using Godot;
using System;

public class ShootingStar : KinematicBody2D
{
    [Export]
    private float _maxSpeed = 100;
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
            
            // shooting star should only move horizontally
            //_velocity = new Vector2(_velocity.x, 0);
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
