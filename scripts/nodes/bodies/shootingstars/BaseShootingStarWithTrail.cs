using Godot;
using System;

public class BaseShootingStarWithTrail : BaseShootingStar
{
    public override void _Ready()
    {
        base._Ready();
        GetNode<Line2D>("Trail").Width = Radius;
    }

    public override void HideAll()
    {
        Hide();
        GetNode<Line2D>("Trail").Hide();
    }

    public override void ShowAll()
    {
        Show();
        GetNode<Line2D>("Trail").Show();
    }

    public override void Disintegrate()
    {
        GetNode<Line2D>("Trail").Hide();
        base.Disintegrate();
    }
}
