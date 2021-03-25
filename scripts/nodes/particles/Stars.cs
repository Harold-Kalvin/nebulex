using Godot;
using System;

[Tool]
public class Stars : Particles2D
{
    public override void _Ready()
    {
        ParticlesMaterial material = (ParticlesMaterial)ProcessMaterial;
        // particles emission range
        material.EmissionBoxExtents = new Vector3(Screen.Size.x / 2, 1, 1);
        // emission range centered
        Position = new Vector2(Position.x + Screen.Position.x, 0);
    }

    public void Pause() {
        SpeedScale = 0;
    }
}
