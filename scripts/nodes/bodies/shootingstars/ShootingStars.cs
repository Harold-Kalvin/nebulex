using Godot;
using System;
using System.Collections.Generic;

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
        get => _shootingStars[Role.Current];
    }

    private Vector2 _screenSize;
    private Timer _idleTimer;
    private PackedScene _shootingStarScene = GD.Load<PackedScene>("res://scenes/bodies/shootingstars/BaseShootingStarWithTrail.tscn");
    private Dictionary<Role, BaseShootingStar> _shootingStars = new Dictionary<Role, BaseShootingStar>();
    private BaseShootingStar _leftClone {
        get => _shootingStars[Role.LeftClone];
    }
    private BaseShootingStar _rightClone {
        get => _shootingStars[Role.RightClone];
    }
    private bool _destroyed = false;

    public override void _Ready()
    {
        _screenSize = GetViewport().GetVisibleRect().Size;
        
        // time before idle mode
        _idleTimer = (Timer)GetNode("IdleTimer");
        _idleTimer.Connect("timeout", this, nameof(_OnIdleTimerTimout));

        // the "current" shooting star is the one visible on screen
        _shootingStars[Role.Current]  = (BaseShootingStar)_shootingStarScene.Instance();
        ShootingStar.SetCameraCurrent();
        AddChild(ShootingStar);
        
        // the "clones" are the ones outside the screen
        // if the current one leaves the screen, one of the clones takes its place
        // it creates a "teleport" effect: the shooting star leaves the screen
        // from one edge to reappear at the other
        _shootingStars[Role.LeftClone] = _CloneShootingStar(new Vector2(-_screenSize.x, 0));
        _shootingStars[Role.RightClone] = _CloneShootingStar(new Vector2(_screenSize.x, 0));
        _leftClone.HideAll();
        _rightClone.HideAll();
        AddChild(_leftClone);
        AddChild(_rightClone);
    }

    public override void _Process(float delta)
    {   
        if (_destroyed) {
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

    public void Sprint()
    {
        if (_destroyed) {
            return;
        }

        var farForward = ShootingStar.Position.y - _screenSize.x * 0.5f;
        _SetTarget(new Vector2(ShootingStar.Position.x, farForward));
        _SetCanBeIdle(true);
    }

    public void MoveLeft()
    {
        if (_destroyed) {
            return;
        }

        var farLeft = ShootingStar.Position.x - _screenSize.x * 0.25f;
        _SetTarget(new Vector2(farLeft, ShootingStar.Position.y));
        _SetDirection(Direction.Left);
        _SetCanBeIdle(false);
        _idleTimer.Start();
    }

    public void MoveRight()
    {
        if (_destroyed) {
            return;
        }

        var farRight = ShootingStar.Position.x + _screenSize.x * 0.25f;
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
        return clone;
    }

    private Edge _NearEdges()
    {
        var screenPos = ShootingStar.GetGlobalTransformWithCanvas()[2].x;
        var limitX = _screenSize.x * 0.25;
        if (screenPos < limitX)
        {
            return Edge.Left;
        }
        else if (screenPos > _screenSize.x - limitX)
        {
            return Edge.Right;
        }
        return Edge.None;
    }

    private bool _OutsideEdges()
    {
        var screenPos = ShootingStar.GetGlobalTransformWithCanvas()[2].x;
        return screenPos + ShootingStar.Radius < 0 || screenPos - ShootingStar.Radius > _screenSize.x;
    }

    private void _replaceCurrentWithClone(Edge exitSide)
    {
        BaseShootingStar currentTemp = ShootingStar;
        if (exitSide == Edge.Left)
        {
            BaseShootingStar leftTemp = _leftClone;
            _shootingStars[Role.Current] = _rightClone;
            _shootingStars[Role.LeftClone] = currentTemp;
            _shootingStars[Role.RightClone] = leftTemp;
            _rightClone.Position = new Vector2(_rightClone.Position.x + _screenSize.x * 3, _rightClone.Position.y);
            _rightClone.Target = new Vector2(_rightClone.Target.x + _screenSize.x * 3, _rightClone.Target.y);
        }
        else if (exitSide == Edge.Right)
        {
            BaseShootingStar rightTemp = _rightClone;
            _shootingStars[Role.Current] = _leftClone;
            _shootingStars[Role.RightClone] = currentTemp;
            _shootingStars[Role.LeftClone] = rightTemp;
            _leftClone.Position = new Vector2(_leftClone.Position.x - _screenSize.x * 3, _leftClone.Position.y);
            _leftClone.Target = new Vector2(_leftClone.Target.x - _screenSize.x * 3, _leftClone.Target.y);
        }
        ShootingStar.SetCameraCurrent();
    }

    private void _OnIdleTimerTimout()
    {
        _SetCanBeIdle(true);
    }
}
