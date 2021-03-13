using Godot;
using System;

public class Game : Node {
    [Signal]
    public delegate void Over();

    public void GameOver() {
        EmitSignal(nameof(Over));
    }
}