using Godot;
using System;

public class ShootingStar : Polygon2D
{
    [Export]
    private float _maxSpeed = 60;
    [Export]
    private float _seekForce = 0.5F;

    public float Radius;
    public Vector2 PositionToFollow;

    private Vector2 _velocity;

    public override void _Ready()
    {
        // setting radius from screen size
        Radius = (GetViewport().GetVisibleRect().Size.x * 0.05F) / 2;
        GetNode<Line2D>("Trail").Width = Radius;
        var collisionShape = (CircleShape2D)GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
        collisionShape.Radius = Radius;

        // connecting collision signal
        GetNode<Area2D>("Area2D").Connect("area_entered", this, "_OnObstacleEntered");

        // init position to follow
        PositionToFollow.y = -350;
    }

    public override void _Draw()
    {   
        DrawCircle(new Vector2(0, 0), Radius, new Color("#FFFFFF"));
    }

    public override void _Process(float delta)
    {
        var acceleration = _Seek(PositionToFollow);
        _velocity += acceleration * delta;
        _velocity.y = Num.Clamp(_velocity.y, -_maxSpeed, -_maxSpeed / 3); // keep moving forward
        Translate(_velocity);
    }

    private Vector2 _Seek(Vector2 target)
    {
        var desired = (target - Position).Normalized() * _maxSpeed;
        var steer = desired - _velocity;
        return steer * _seekForce;
    }

    private void _OnObstacleEntered(Node2D body)
    {
        GD.Print($"collided with body: {body}");
    }
}
