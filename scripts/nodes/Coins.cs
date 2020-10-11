using Godot;
using System;

public class Coins : Node2D
{
    private Vector2 _screenSize;
    private PackedScene _smallCoinScene = GD.Load<PackedScene>("res://scenes/SmallCoin.tscn");
    private Planets _planets;
    private int _planetsCount = 0;
    private int _smallCoinsEveryOtherPlanets = 2;

    public override void _Ready()
    {
        _screenSize = GetViewport().GetVisibleRect().Size;    
        _planets = (Planets)GetNode("../Planets");
        _planets.Connect("BigPlanetCreated", this, nameof(_OnBigPlanetCreated));
    }

    private void _GenerateSmallCoins(Vector2 centerPos, int total = 8)
    {
        if (total <= 0)
        {
            return;
        }
        
        // set of coins centered vertically
        int remaining = total;
        float startPosToTopY;
        float startPosToBottomY;
        float startPosOffset = 0;
        if (total % 2 != 0)
        {
            SmallCoin smallCoin = (SmallCoin)_smallCoinScene.Instance();
            smallCoin.Position = centerPos;
            AddChild(smallCoin);
            startPosOffset = smallCoin.Radius * 1.5f;
            remaining--;
        }
        for (int i = 0; i < remaining / 2; i++)
        {
            SmallCoin smallCoin = (SmallCoin)_smallCoinScene.Instance();
            AddChild(smallCoin);
            startPosToTopY = centerPos.y - (smallCoin.Radius * 1.5f) - startPosOffset;
            float positionOffset = (smallCoin.Radius * 3f) * i;
            smallCoin.Position = new Vector2(centerPos.x, startPosToTopY - positionOffset);
        }
        for (int i = 0; i < remaining / 2; i++)
        {
            SmallCoin smallCoin = (SmallCoin)_smallCoinScene.Instance();
            AddChild(smallCoin);
            startPosToBottomY = centerPos.y + (smallCoin.Radius * 1.5f) + startPosOffset;
            float positionOffset = (smallCoin.Radius * 3f) * i;
            smallCoin.Position = new Vector2(centerPos.x, startPosToBottomY + positionOffset);
        }
    }

    private void _OnBigPlanetCreated(Planet planet)
    {
        _planetsCount++;

        // generate small coins every other big planets
        if (_planetsCount % _smallCoinsEveryOtherPlanets == 0)
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
            _GenerateSmallCoins(new Vector2(x, y), 8);
        }
        
        // reinit counter
        if (_planetsCount == 1000)
        {
            _planetsCount = 0;
        }
    }
}
