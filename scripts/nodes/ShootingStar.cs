using Godot;
using System;

public class ShootingStar : Polygon2D
{
    [Export]
    private float _maxSpeed = 60;
    [Export]
    private float _seekForce = 1.5F;
    [Export]
    private float _oscillationAmplitude = 0.5f;
    [Export]
    private float _oscillationAngle = 0;
    [Export]
    private float _oscillationAngularVelocity = 0.05f;

    public float Radius;
    public Vector2 PositionToFollow;

    private Vector2 _velocity;
    private float _minSpeed;

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

        // init min speed
        _minSpeed = _maxSpeed / 3;
    }

    public override void _Draw()
    {   
        DrawCircle(new Vector2(0, 0), Radius, new Color("#FFFFFF"));
    }

    public override void _Process(float delta)
    {
        var acceleration = _Seek(PositionToFollow);
        _velocity += acceleration * delta;
        _velocity.y = Num.Clamp(_velocity.y, -_maxSpeed, -_minSpeed); // keep moving forward
        _Oscillate(); // add some horizontal oscillation
        Translate(_velocity);
    }

    public void HideWithChildren()
    {
        Hide();
        GetNode<Line2D>("Trail").Hide();
    }

    public void ShowWithChildren()
    {
        Show();
        GetNode<Line2D>("Trail").Show();
    }

    public void SetCameraCurrent()
    {
        GetNode<Camera2D>("Camera").Current = true;
    }

    private Vector2 _Seek(Vector2 target)
    {
        var desired = (target - Position).Normalized() * _maxSpeed;
        var steer = desired - _velocity;
        return steer * _seekForce;
    }

    private void _Oscillate()
    {
        var x = _oscillationAmplitude * Mathf.Cos(_oscillationAngle);
        var target = new Vector2(x, 0);
        _oscillationAngle += _oscillationAngularVelocity;
        _velocity.x = x;
    }

    private void _OnObstacleEntered(Node2D body)
    {
        GD.Print($"collided with body: {body}");
    }
}
