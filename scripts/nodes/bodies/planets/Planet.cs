using Godot;
using System;

public class Planet : Sprite
{
    public const int CIRCLE_RADIUS = 128;
    public const float FADE_DURATION = 0.5f;
    public float Radius {
        get => _radius;
        set {
            _radius = value;
            var scaleComponent = _radius / CIRCLE_RADIUS;
            Scale = new Vector2(scaleComponent, scaleComponent);
        }
    }
    public Vector2 BarycenterPosition;
    public float OrbitalSpeed = 1;

    private float _radius;
    private float _distanceToBarycenter;
    private float _angleToBarycenter;

    public override void _Ready()
    {
        AddToGroup("bodies");
        
        // if barycenter exists, set related attributes
        if (BarycenterPosition != new Vector2(0, 0))
        {
            _distanceToBarycenter = Position.DistanceTo(BarycenterPosition);
            _angleToBarycenter = Position.AngleTo(BarycenterPosition);
            // randomize orbital direction (clockwise, counterclockwise)
            if ((float)GD.RandRange(0, 1) > 0.5)
            {
                OrbitalSpeed = -OrbitalSpeed;
            }
        }
    }

    public override void _Process(float delta)
    {
        // if barycenter exists, make orbital movements around it
        if (BarycenterPosition != new Vector2(0, 0))
        {
            _angleToBarycenter += OrbitalSpeed * delta;
            var offset = new Vector2(Mathf.Sin(_angleToBarycenter), Mathf.Cos(_angleToBarycenter)) * _distanceToBarycenter;
            Position = BarycenterPosition + offset;
        }
    }

    public async void Fade() {
        var fadeTween = GetNode<Tween>("FadeTween");
        Color transparent = new Color(Modulate.r, Modulate.g, Modulate.b, 0);
        fadeTween.InterpolateProperty(this, "modulate", Modulate, transparent, FADE_DURATION);
        fadeTween.Start();
        await ToSignal(fadeTween, "tween_completed");
        QueueFree();
    }
}
