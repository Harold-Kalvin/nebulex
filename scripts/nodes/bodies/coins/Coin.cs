using Godot;
using System;

public class Coin : Node2D
{
    public const float CIRCLE_RADIUS = 129.5f;
    public float Radius {
        get => _radius;
    }

    private float _radius;
    private float _coinMinSizeScreenX = 0.12f;
    private float _coinMaxSizeScreenX = 0.18f;

    public override void _Ready()
    {
        AddToGroup("bodies");

        // setting radius from screen size
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        _radius = (float)GD.RandRange(
            (screenSize.x * _coinMinSizeScreenX) / 2,
            (screenSize.x * _coinMaxSizeScreenX) / 2
        );
        var scaleComponent = _radius / CIRCLE_RADIUS;
        Scale = new Vector2(scaleComponent, scaleComponent);
    }

    public async void Explode()
    {
        // hide body
        GetNode<Sprite>("Blur").Hide();
        GetNode<Sprite>("Body").Hide();
        // emit particles
        await GetNode<CustomParticles>("Explosion").Emit();
        QueueFree();
    }
}
