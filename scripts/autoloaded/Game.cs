using Godot;
using System;

public class Game : Node {
    [Signal]
    public delegate void Over();

    public bool Started {
        get => _started;
    }

    public bool _started = false;

    public void Start() {
        _started = true;
    }

    public void GameOver() {
        _started = false;
        EmitSignal(nameof(Over));
    }
}