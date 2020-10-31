using Godot;
using System;

public enum Direction
{
    Left,
    Right,
}

public class ShootingStar : Polygon2D
{
    [Export]
    private float _sizeScreenX = 0.045f;
    [Export]
    private float _maxSpeed = 40;
    [Export]
    private float _seekForce = 2.5F;
    [Export]
    private float _oscillationAmplitude = 0.6f;
    [Export]
    private float _oscillationAngle = 0;
    [Export]
    private float _oscillationAngularVelocity = 0.05f;

    public float Radius;
    public Vector2 Target = new Vector2(0, -1);
    public Direction LastDirection;
    public bool CanBeIdle {
        get => _canBeIdle;
        set {
            _canBeIdle = value;
            if (!_canBeIdle)
            {
                _isIdle = false;
            }
        }
    }

    private Vector2 _velocity;
    private float _minSpeed;
    private bool _canBeIdle = true;
    private bool _isIdle = true;

    public override void _Ready()
    {
        // setting radius from screen size
        Radius = (GetViewport().GetVisibleRect().Size.x * _sizeScreenX) / 2;
        GetNode<Line2D>("Trail").Width = Radius;
        var collisionShape = (CircleShape2D)GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
        collisionShape.Radius = Radius;

        // connecting collision signal
        GetNode<Area2D>("Area2D").Connect("area_entered", this, "_OnObstacleEntered");

        // init min speed
        _minSpeed = _maxSpeed / 3;
    }

    public override void _Draw()
    {   
        DrawCircle(new Vector2(0, 0), Radius, new Color("#FFFFFF"));
    }

    public override void _Process(float delta)
    {
        // move (on sides)
        _Seek(delta);

        // keep moving forward
        _velocity.y = Num.Clamp(_velocity.y, -_maxSpeed, -_minSpeed);
        
        // add some horizontal oscillation if needed
        if (!_isIdle)
        {
            if (CanBeIdle)
            {
                var comingBackFromLeft = LastDirection == Direction.Left && _velocity.x > 0;
                var comingBackFromRight = LastDirection == Direction.Right && _velocity.x < 0;
                if (comingBackFromLeft || comingBackFromRight)
                {
                    _isIdle = true;
                    _oscillationAmplitude = comingBackFromLeft ? Mathf.Abs(_oscillationAmplitude) : -Mathf.Abs(_oscillationAmplitude);
                    _oscillationAngle = 0;
                }
            }
        }
        else
        {
            _Oscillate();
        }
        
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

    private void _Seek(float delta)
    {
        var desired = (Target - Position).Normalized() * _maxSpeed;
        var steer = desired - _velocity;
        var acceleration = steer * _seekForce;
        _velocity += acceleration * delta;
    }

    private void _Oscillate()
    {
        var x = _oscillationAmplitude * Mathf.Cos(_oscillationAngle);
        _oscillationAngle += _oscillationAngularVelocity;
        _velocity.x = x;
    }

    private void _OnObstacleEntered(Node2D body)
    {
        // if collided with coin
        var coin = body.GetParent() as Coin;
        if (coin != null)
        {
            coin.Explode();
        }
    }
}
