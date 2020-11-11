using Godot;
using System;

[Tool]
public class Stars : Particles2D
{
    public override void _Ready()
    {
        Vector2 _screenSize = GetViewport().GetVisibleRect().Size;
        ParticlesMaterial material = (ParticlesMaterial)ProcessMaterial;
        // particles emission range
        material.EmissionBoxExtents = new Vector3(_screenSize.x / 2, 1, 1);
        // emission range centered
        Position = new Vector2(_screenSize.x / 2, 0);
    }
}
