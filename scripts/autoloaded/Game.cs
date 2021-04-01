using Godot;
using System;
using System.Threading.Tasks;

public class Game : Node {
    [Signal]
    public delegate void Over();

    public bool Started {
        get => _started;
    }

    private bool _started = false;

    public async Task Wait() {
        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
    }

    public void Start() {
        _started = true;
    }

    public void GameOver() {
        _started = false;
        EmitSignal(nameof(Over));
    }
}