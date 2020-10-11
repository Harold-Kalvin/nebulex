using Godot;
using System;

public class Planet : Polygon2D
{
    public float Radius {
        get => _radius;
        set {
            _radius = value;
            // set collision shape from radius
            var collisionShape = (CircleShape2D)GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
            collisionShape.Radius = _radius;
            Update();
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

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), _radius, new Color("#6A0DAD"));
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
}
