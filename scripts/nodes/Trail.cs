using Godot;
using System.Collections.Generic;


public class Trail : Line2D
{
    [Export]
    private NodePath _targetPath;
    [Export]
    private bool _isEmitting = false;
    [Export]
    private float _resolution = 5.0F;
    [Export]
    private float _lifetime = 0.5F;
    [Export]
    private float _maxPoints = 10;

    public bool IsEmitting
    {
        set => SetIsEmitting(value);
    }
    public Node2D Target;

    private List<float> _pointsCreationTime = new List<float>();
    private Vector2 _lastPoint;
    private float _clock = 0.0F;
    private float _offset = 0.0F;

    public override void _Ready()
    {
        Target = (_targetPath != null) ? (Node2D)GetNodeOrNull(_targetPath) : (Node2D)GetParent();

        if (Engine.EditorHint)
        {
            SetProcess(false);
            return;
        }

        _offset = Position.Length();
        SetAsToplevel(true);
        ClearPoints();
        Position = new Vector2();
        _lastPoint = ToLocal(Target.GlobalPosition) + CalculateOffset();
    }

    public override void _Process(float delta)
    {
        _clock += delta;
        RemoveOlder();

        if (!_isEmitting)
        {
            return;
        }

        var desiredPoint = ToLocal(Target.GlobalPosition);
        float distance = _lastPoint.DistanceTo(desiredPoint);
        if (distance > _resolution)
        {
            AddTimePoint(desiredPoint, _clock);
        } 
    }

    public void AddTimePoint(Vector2 point, float time)
    {
        AddPoint(point + CalculateOffset());
        _pointsCreationTime.Add(time);
        _lastPoint = point;
        if (GetPointCount() > _maxPoints)
        {
            RemoveFirstPoint();
        }
    }

    public Vector2 CalculateOffset()
    {
        return -1.0F * Mathf.Polar2Cartesian(1.0F, Target.Rotation).Rotated(-Mathf.Pi / 2) * _offset;
    }

    public void RemoveFirstPoint()
    {
        if (GetPointCount() > 1)
        {
            RemovePoint(0);
        }
        _pointsCreationTime.RemoveAt(0);
    }

    public void RemoveOlder()
    {
        for (int i = 0; i < _pointsCreationTime.Count; i++)
        {
            var creationTime = _pointsCreationTime[i];
            var delta = _clock - creationTime;
            if (delta > _lifetime)
            {
                RemoveFirstPoint();
            }
            else
            {
                break;
            }
        }
    }

    public async void SetIsEmitting(bool value)
    {
        _isEmitting = value;
        if (Engine.EditorHint)
        {
            return;
        }

        if (!IsInsideTree())
        {
            await ToSignal(this, "ready");
        }

        if (_isEmitting)
        {
            ClearPoints();
            _pointsCreationTime.Clear();
            _lastPoint = ToLocal(Target.GlobalPosition) + CalculateOffset();
        }
    }
}
