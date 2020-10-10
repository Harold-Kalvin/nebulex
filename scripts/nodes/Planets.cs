using Godot;
using System;
using System.Collections.Generic;

public class Planets : Node2D
{
    private Vector2 _screenSize;
    private PackedScene _planetScene = GD.Load<PackedScene>("res://scenes/Planet.tscn");
    private List<Planet> _planets = new List<Planet>();
    // planet size
    private float _bigPlanetMinSizeScreenX = 0.25f;
    private float _bigPlanetMaxSizeScreenX = 0.35f;
    private float _smallPlanetMinSizeScreenX = 0.1f;
    private float _smallPlanetMaxSizeScreenX = 0.15f;
    private float _smallPlanetApparitionRate = 0.2f;
    // planet position
    private float _bigPlanetMinVerticalDistanceScreenX = 0.65f;
    private float _bigPlanetMaxVerticalDistanceScreenX = 0.75f;

    public override void _Ready()
    {
        GD.Randomize();

        _screenSize = GetViewport().GetVisibleRect().Size;
    }

    public override void _Process(float delta)
    {
        if (_planets.Count == 0)
        {
            _GenerateBigPlanet();
        }
        else
        {
            // wait until the last planet has reached a certain point
            // before generating another one (for performance) 
            Planet lastPlanet = _planets[_planets.Count - 1];
            if (lastPlanet.GetGlobalTransformWithCanvas()[2].y > -_screenSize.y / 2)
            {
                _GenerateBigPlanet();
            }
        }
        GD.Print(_planets.Count);
    }
    
    public void Remove(Planet planet)
    {
        if (_planets.Contains(planet)) {
            _planets.Remove(planet);
        }
    }

    private void _GenerateBigPlanet()
    {
        Planet planet = (Planet)_planetScene.Instance();
        planet.Radius = _GeneratePlanetRadius(PlanetType.Big);
        planet.Position = _planets.Count == 0 ? _GenerateFirstBigPlanetPosition(planet.Radius) : _GenerateBigPlanetPosition(planet.Radius);
        _planets.Add(planet);
        AddChild(planet);
    }

    private Vector2 _GenerateFirstBigPlanetPosition(float planetRadius)
    {
        // random horizontal position
        var x = (float)GD.RandRange((-_screenSize.x / 2) + planetRadius, (_screenSize.x / 2) - planetRadius);
        // screen height vertical position
        var y = -(GetCanvasTransform().Xform(new Vector2(0, _screenSize.y))).y;
        return new Vector2(x, y);
    }

    private Vector2 _GenerateBigPlanetPosition(float planetRadius)
    {
        // random horizontal position
        var x = (float)GD.RandRange((-_screenSize.x / 2) + planetRadius, (_screenSize.x / 2) - planetRadius);
        // vertical position from last planet
        Planet lastPlanet = _planets[_planets.Count - 1];
        float lastPlanetVerticalLimit = (lastPlanet.Position.y - lastPlanet.Radius);
        float minVerticalDistance = _screenSize.x * _bigPlanetMinVerticalDistanceScreenX;
        float maxVerticalDistance = _screenSize.x * _bigPlanetMaxVerticalDistanceScreenX;
        float verticalDistance = (float)GD.RandRange(minVerticalDistance, maxVerticalDistance);
        var y = lastPlanetVerticalLimit - verticalDistance;
        return new Vector2(x, y);
    }
    
    private float _GeneratePlanetRadius(PlanetType type)
    {
        var screenSize = GetViewport().GetVisibleRect().Size.x;
        if (type == PlanetType.Big)
        {
            return (float)GD.RandRange(
                (screenSize * _bigPlanetMinSizeScreenX) / 2,
                (screenSize * _bigPlanetMaxSizeScreenX) / 2
            );
        }
        return (float)GD.RandRange(
            (screenSize * _smallPlanetMinSizeScreenX) / 2,
            (screenSize * _smallPlanetMaxSizeScreenX) / 2
        );
    }
}