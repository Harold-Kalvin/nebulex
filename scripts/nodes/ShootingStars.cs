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
    public ShootingStar ShootingStar {
        get => _shootingStars[Role.Current];
    }

    private Vector2 _screenSize;
    private PackedScene _shootingStarScene = GD.Load<PackedScene>("res://scenes/ShootingStar.tscn");
    private Dictionary<Role, ShootingStar> _shootingStars = new Dictionary<Role, ShootingStar>();
    private ShootingStar _leftClone {
        get => _shootingStars[Role.LeftClone];
    }
    private ShootingStar _rightClone {
        get => _shootingStars[Role.RightClone];
    }

    public override void _Ready()
    {
        _screenSize = GetViewport().GetVisibleRect().Size;
        // the "current" shooting star is the one visible on screen
        _shootingStars[Role.Current]  = (ShootingStar)_shootingStarScene.Instance();
        ShootingStar.SetCameraCurrent();
        AddChild(ShootingStar);
        // the "clones" are the ones outside the screen
        // if the current one leaves the screen, one of the clones takes its place
        // it creates a "teleport" effect: the shooting star leaves the screen
        // from one edge to reappear at the other
        _shootingStars[Role.LeftClone] = _CloneShootingStar(new Vector2(-_screenSize.x, 0));
        _shootingStars[Role.RightClone] = _CloneShootingStar(new Vector2(_screenSize.x, 0));
        _leftClone.HideWithChildren();
        _rightClone.HideWithChildren();
        AddChild(_leftClone);
        AddChild(_rightClone);
    }

    public override void _Process(float delta)
    {
        // hide the clones if the current one isn't near the edges (for performance)
        if (_NearEdges() == Edge.None)
        {
            _leftClone.HideWithChildren();
            _rightClone.HideWithChildren();
        }
        // show back the clones if the current one is near the edges
        // then replaces the current one with a clone if it leaves the screen
        else if (_NearEdges() == Edge.Left)
        {
            _rightClone.ShowWithChildren();
            if (_OutsideEdges())
            {
                _replaceCurrentWithClone(Edge.Left);
            }
        }
        else if (_NearEdges() == Edge.Right)
        {
            _leftClone.ShowWithChildren();
            if (_OutsideEdges())
            {
                _replaceCurrentWithClone(Edge.Right);
            }
        }
    }

    public void Sprint()
    {
        var leftCloneOffset = _leftClone.Position - ShootingStar.Position;
        var rightCloneOffset = _rightClone.Position - ShootingStar.Position;
        ShootingStar.PositionToFollow = new Vector2(ShootingStar.Position.x, ShootingStar.Position.y - _screenSize.x * 1.5f);
        _leftClone.PositionToFollow = ShootingStar.PositionToFollow + leftCloneOffset;
        _rightClone.PositionToFollow = ShootingStar.PositionToFollow + rightCloneOffset;
    }

    private ShootingStar _CloneShootingStar(Vector2 offsetPosition)
    {
        var clone = (ShootingStar)_shootingStarScene.Instance();
        clone.Position = ShootingStar.Position + offsetPosition;
        clone.PositionToFollow = ShootingStar.PositionToFollow + offsetPosition;
        return clone;
    }

    private Edge _NearEdges()
    {
        var screenPos = ShootingStar.GetGlobalTransformWithCanvas()[2].x;
        var limitX = _screenSize.x * 0.1;
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
        ShootingStar currentTemp = ShootingStar;
        if (exitSide == Edge.Left)
        {
            ShootingStar leftTemp = _leftClone;
            _shootingStars[Role.Current] = _rightClone;
            _shootingStars[Role.LeftClone] = currentTemp;
            _shootingStars[Role.RightClone] = leftTemp;
            _rightClone.Position = new Vector2(_rightClone.Position.x + _screenSize.x * 3, _rightClone.Position.y);
            _rightClone.PositionToFollow = new Vector2(_rightClone.PositionToFollow.x + _screenSize.x * 3, _rightClone.PositionToFollow.y);
        }
        else if (exitSide == Edge.Right)
        {
            ShootingStar rightTemp = _rightClone;
            _shootingStars[Role.Current] = _leftClone;
            _shootingStars[Role.RightClone] = currentTemp;
            _shootingStars[Role.LeftClone] = rightTemp;
            _leftClone.Position = new Vector2(_leftClone.Position.x - _screenSize.x * 3, _leftClone.Position.y);
            _leftClone.PositionToFollow = new Vector2(_leftClone.PositionToFollow.x - _screenSize.x * 3, _leftClone.PositionToFollow.y);
        }
        ShootingStar.SetCameraCurrent();
    }
}
