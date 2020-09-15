using Godot;

public class Trail : Line2D
{
    [Export]
    private NodePath _targetPath;
    [Export]
    private bool _isEmitting = false;
    [Export]
    private float _resolution = 5.0F;
    [Export]
    private float _maxPoints = 100;

    public bool IsEmitting
    {
        set => SetIsEmitting(value);
    }
    public Node2D Target;

    private Vector2 _lastPoint;
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
        if (!_isEmitting)
        {
            return;
        }

        var desiredPoint = ToLocal(Target.GlobalPosition);
        float distance = _lastPoint.DistanceTo(desiredPoint);
        if (distance > _resolution)
        {
            AddTimePoint(desiredPoint);
        } 
    }

    public void AddTimePoint(Vector2 point)
    {
        AddPoint(point + CalculateOffset());
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
            _lastPoint = ToLocal(Target.GlobalPosition) + CalculateOffset();
        }
    }
}
