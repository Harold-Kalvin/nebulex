using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

enum Edge {
    None,
    Left,
    Right,
}

enum Role {
    Current,
    LeftClone,
    RightClone,
}

public class ShootingStars : Node2D
{
    public BaseShootingStar ShootingStar {
        get => _shootingStars.ContainsKey(Role.Current) ? _shootingStars[Role.Current] : null;
    }
    private Timer _idleTimer;
    private PackedScene _shootingStarScene = GD.Load<PackedScene>("res://scenes/bodies/shootingstars/BaseShootingStarWithTrail.tscn");
    private Dictionary<Role, BaseShootingStar> _shootingStars = new Dictionary<Role, BaseShootingStar>();
    private BaseShootingStar _leftClone {
        get => _shootingStars.ContainsKey(Role.LeftClone) ? _shootingStars[Role.LeftClone] : null;
    }
    private BaseShootingStar _rightClone {
        get => _shootingStars.ContainsKey(Role.RightClone) ? _shootingStars[Role.RightClone] : null;
    }
    private bool _destroyed = false;
    private bool _initialized {
        get => ShootingStar != null && _leftClone != null && _rightClone != null;
    }

    public override void _Ready()
    {        
        // time before idle mode
        _idleTimer = (Timer)GetNode("IdleTimer");
        _idleTimer.Connect("timeout", this, nameof(_OnIdleTimerTimout));
    }

    public override void _Process(float delta)
    {   
        if (_destroyed || !_initialized) {
            return;
        }

        if (ShootingStar.Destroyed) {
            _destroyed = true;
            if (IsInstanceValid(_leftClone)) {
                _leftClone.QueueFree();
            }
            if (IsInstanceValid(_rightClone)) {
                _rightClone.QueueFree();
            }
        }

        // hide the clones if the current one isn't near the edges (for performance)
        if (_NearEdges() == Edge.None)
        {
            _leftClone.HideAll();
            _rightClone.HideAll();
        }
        // show back the clones if the current one is near the edges
        // then replaces the current one with a clone if it leaves the screen
        else if (_NearEdges() == Edge.Left)
        {
            _rightClone.ShowAll();
            if (_OutsideEdges())
            {
                _replaceCurrentWithClone(Edge.Left);
            }
        }
        else if (_NearEdges() == Edge.Right)
        {
            _leftClone.ShowAll();
            if (_OutsideEdges())
            {
                _replaceCurrentWithClone(Edge.Right);
            }
        }
    }

    public async Task Init() {
        if (IsInstanceValid(ShootingStar)) {
            ShootingStar.QueueFree();
        }

        // the "current" shooting star is the one visible on screen
        _shootingStars[Role.Current]  = (BaseShootingStar)_shootingStarScene.Instance();
        AddChild(ShootingStar);
        ShootingStar.SetCameraCurrent();
        await ShootingStar.MoveCamera();
        
        // the "clones" are the ones outside the screen
        // if the current one leaves the screen, one of the clones takes its place
        // it creates a "teleport" effect: the shooting star leaves the screen
        // from one edge to reappear at the other
        _shootingStars[Role.LeftClone] = _CloneShootingStar(new Vector2(-Screen.Size.x, 0));
        _shootingStars[Role.RightClone] = _CloneShootingStar(new Vector2(Screen.Size.x, 0));
        _leftClone.HideAll();
        _rightClone.HideAll();
        AddChild(_leftClone);
        AddChild(_rightClone);
    }

    public void MoveLeft()
    {
        if (_destroyed || !_initialized) {
            return;
        }

        var farLeft = ShootingStar.Position.x - Screen.Size.x * 0.25f;
        _SetTarget(new Vector2(farLeft, ShootingStar.Position.y));
        _SetDirection(Direction.Left);
        _SetCanBeIdle(false);
        _idleTimer.Start();
    }

    public void MoveRight()
    {
        if (_destroyed || !_initialized) {
            return;
        }

        var farRight = ShootingStar.Position.x + Screen.Size.x * 0.25f;
        _SetTarget(new Vector2(farRight, ShootingStar.Position.y));
        _SetDirection(Direction.Right);
        _SetCanBeIdle(false);
        _idleTimer.Start();
    }

    private void _SetTarget(Vector2 target)
    {
        ShootingStar.Target = target;
        // set clones' target
        var leftCloneOffset = _leftClone.Position - ShootingStar.Position;
        var rightCloneOffset = _rightClone.Position - ShootingStar.Position;
        _leftClone.Target = ShootingStar.Target + leftCloneOffset;
        _rightClone.Target = ShootingStar.Target + rightCloneOffset;
    }

    private void _SetDirection(Direction direction)
    {
        ShootingStar.LastDirection = direction;
        _leftClone.LastDirection = direction;
        _rightClone.LastDirection = direction;
    }

    private void _SetCanBeIdle(bool value)
    {
        ShootingStar.CanBeIdle = value;
        _leftClone.CanBeIdle = value;
        _rightClone.CanBeIdle = value;
    }

    private BaseShootingStar _CloneShootingStar(Vector2 offsetPosition)
    {
        var clone = (BaseShootingStar)_shootingStarScene.Instance();
        clone.Position = ShootingStar.Position + offsetPosition;
        clone.Target = ShootingStar.Target + offsetPosition;
        clone.Current = false;
        return clone;
    }

    private Edge _NearEdges()
    {
        var screenPos = ShootingStar.GetGlobalTransformWithCanvas()[2].x - Screen.Position.x;
        var limitX = Screen.Size.x * 0.25;
        if (screenPos < limitX)
        {
            return Edge.Left;
        }
        else if (screenPos > Screen.Size.x - limitX)
        {
            return Edge.Right;
        }
        return Edge.None;
    }

    private bool _OutsideEdges()
    {
        var screenPos = ShootingStar.GetGlobalTransformWithCanvas()[2].x - Screen.Position.x;
        return screenPos + ShootingStar.Radius < 0 || screenPos - ShootingStar.Radius > Screen.Size.x;
    }

    private void _replaceCurrentWithClone(Edge exitSide)
    {
        ShootingStar.Current = false;
        BaseShootingStar currentTemp = ShootingStar;
        if (exitSide == Edge.Left)
        {
            BaseShootingStar leftTemp = _leftClone;
            _shootingStars[Role.Current] = _rightClone;
            _shootingStars[Role.LeftClone] = currentTemp;
            _shootingStars[Role.RightClone] = leftTemp;
            _rightClone.Position = new Vector2(_rightClone.Position.x + Screen.Size.x * 3, _rightClone.Position.y);
            _rightClone.Target = new Vector2(_rightClone.Target.x + Screen.Size.x * 3, _rightClone.Target.y);
        }
        else if (exitSide == Edge.Right)
        {
            BaseShootingStar rightTemp = _rightClone;
            _shootingStars[Role.Current] = _leftClone;
            _shootingStars[Role.RightClone] = currentTemp;
            _shootingStars[Role.LeftClone] = rightTemp;
            _leftClone.Position = new Vector2(_leftClone.Position.x - Screen.Size.x * 3, _leftClone.Position.y);
            _leftClone.Target = new Vector2(_leftClone.Target.x - Screen.Size.x * 3, _leftClone.Target.y);
        }
        ShootingStar.Current = true;
        ShootingStar.SetCameraCurrent();
    }

    private void _OnIdleTimerTimout()
    {
        _SetCanBeIdle(true);
    }
}
