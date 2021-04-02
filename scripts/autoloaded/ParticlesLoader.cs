using Godot;
using System.Threading.Tasks;

public class ParticlesLoader : CanvasLayer {

    private CustomParticles _explosion = new CustomParticles();
    private CustomParticles _disintegration = new CustomParticles();

    public override void _Ready() {
        var _explosionMaterial = ResourceLoader.Load("res://materials/Explosion.tres") as Material;
        var _disintegrationMaterial = ResourceLoader.Load("res://materials/Disintegration.tres") as Material;

        // emit particles a first time on autoload to avoid stutters in game
        _EmitParticles(_explosion, _explosionMaterial).ContinueWith((t1) => {
            _explosion.QueueFree();
        });
        _EmitParticles(_disintegration, _disintegrationMaterial).ContinueWith((t1) => {
            _disintegration.QueueFree();
        });
    }

    private async Task _EmitParticles(CustomParticles particles, Material material) {
        AddChild(particles);
        particles.ProcessMaterial = material;
        particles.Modulate = new Color(1, 1, 1, 1);
        particles.OneShot = true;
        await particles.Emit();
    }
}