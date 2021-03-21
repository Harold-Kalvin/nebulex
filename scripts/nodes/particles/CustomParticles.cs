using Godot;
using System;
using System.Threading.Tasks;

public class CustomParticles : Particles2D
{
    public async Task Emit() {
        Emitting = true;
        float _duration = SpeedScale != 0 ? (Lifetime) / SpeedScale : Lifetime;
        await ToSignal(GetTree().CreateTimer(_duration), "timeout");
    }
}
