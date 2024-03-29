using Godot;
using System;

public class GameScene : Node2D
{   
    private Game _game;
    private ShootingStars _shootingStars;
    private Planets _planets;
    private Coins _coins;
    private SwipeDetector _swipeDetector;
    private Stars _stars;

    public async override void _Ready()
    {
        GD.Randomize();

        // get nodes
        _game = (Game)GetNode("/root/Game");
        _shootingStars = (ShootingStars)GetNode("ShootingStars");
        _planets = (Planets)GetNode("Planets");
        _coins = (Coins)GetNode("Coins");
        _swipeDetector = (SwipeDetector)GetNode("SwipeDetector");
        _stars = (Stars)GetNode("CanvasLayer").GetNode("Stars");

        // connect signals
        _game.Connect("Over", this, nameof(_OnGameOver));
        _swipeDetector.Connect("Swiped", this, nameof(_OnSwipe));

        await _game.Wait();  // wait on start to get full animations
        await _shootingStars.Init();
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
