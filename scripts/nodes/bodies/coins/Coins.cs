using Godot;
using System;

public class Coins : Node2D
{
    private Vector2 _screenSize;
    private PackedScene _coinScene = GD.Load<PackedScene>("res://scenes/bodies/Coin.tscn");
    private Planets _planets;
    private int _planetsCount = 0;
    private int _coinsEveryOtherPlanets = 2;

    public override void _Ready()
    {
        _screenSize = GetViewport().GetVisibleRect().Size;    
        _planets = (Planets)GetNode("../Planets");
        _planets.Connect("BigPlanetCreated", this, nameof(_OnBigPlanetCreated));
    }

    private void _GenerateCoin(Vector2 pos)
    {
        Coin smallCoin = (Coin)_coinScene.Instance();
        smallCoin.Position = pos;
        AddChild(smallCoin);
    }

    private void _OnBigPlanetCreated(Planet planet)
    {
        _planetsCount++;

        // generate small coins every other big planets
        if (_planetsCount % _coinsEveryOtherPlanets == 0)
        {
            // random offset to add some randomness in position
            float randomOffset = (float)GD.RandRange(_screenSize.x * -0.1, _screenSize.x * 0.1); 
            
            // y axis: same as planet
            float y = planet.Position.y + randomOffset;

            // x axis: between the planet and the screen edge (side with largest free space) 
            float x = planet.Position.x + randomOffset;
            float planetScreenPos = planet.GetGlobalTransformWithCanvas()[2].x;
            if (planetScreenPos <= _screenSize.x / 2)
            {
                x += ((_screenSize.x - planetScreenPos + planet.Radius) / 2);
            }
            else
            {
                x -= planet.Radius + ((planetScreenPos - planet.Radius) / 2);
            }
            _GenerateCoin(new Vector2(x, y));
        }
        
        // reinit counter
        if (_planetsCount == 1000)
        {
            _planetsCount = 0;
        }
    }
}