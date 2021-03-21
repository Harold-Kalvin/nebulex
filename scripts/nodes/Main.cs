using Godot;
using System;

public class Main : Node2D
{   
    private Game _game;
    private ShootingStars _shootingStars;
    private Planets _planets;
    private Coins _coins;
    private ClickDetector _clickDetector;
    private SwipeDetector _swipeDetector;
    private Stars _stars;

    public override void _Ready()
    {
        GD.Randomize();

        // get nodes
        _game = (Game)GetNode("/root/Game");
        _shootingStars = (ShootingStars)GetNode("ShootingStars");
        _planets = (Planets)GetNode("Planets");
        _coins = (Coins)GetNode("Coins");
        _clickDetector = (ClickDetector)GetNode("ClickDetector");
        _swipeDetector = (SwipeDetector)GetNode("SwipeDetector");
        _stars = (Stars)GetNode("CanvasLayer").GetNode("Stars");

        // connect signals
        _game.Connect("Over", this, nameof(_OnGameOver));
        _clickDetector.Connect("Clicked", this, nameof(_OnClick));
        _swipeDetector.Connect("Swiped", this, nameof(_OnSwipe));

        // start !
        InitGame();
    }

    public override void _Process(float delta)
    {
        // remove bodies that are not visible anymore
        foreach (Node2D body in GetTree().GetNodesInGroup("bodies"))
        {
            var viewportHeight = GetViewport().GetVisibleRect().Size.y;
            var bodyPosition = body.GetGlobalTransformWithCanvas()[2].y;
            if (bodyPosition > viewportHeight * 2)
            {
                body.QueueFree();

                var planet = body as Planet;
                if (planet != null)
                {
                    _planets.Remove(planet);
                }
            }
        }
    }

    public async void InitGame() {
        await _shootingStars.Init();
        _game.Start();
    }

    private void _OnClick()
    {
        if (!_game.Started) {
            return;
        }

        _shootingStars.Sprint();
    }

    private void _OnSwipe(SwipeDirection direction)
    {
        if (!_game.Started) {
            return;
        }

        if (direction == SwipeDirection.Left)
        {
            _shootingStars.MoveLeft();
        }
        else if (direction == SwipeDirection.Right)
        {
            _shootingStars.MoveRight();
        }
    }

    private void _OnGameOver() {
        _stars.Pause();
        _planets.FadeAll();
        _coins.ExplodeAll();
    }
}
