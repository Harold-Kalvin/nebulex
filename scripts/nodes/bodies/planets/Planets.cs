using Godot;
using System;
using System.Collections.Generic;

public class Planets : Node2D
{
    [Signal]
    public delegate void BigPlanetCreated();

    public List<Planet> BigPlanets {
        get => _bigPlanets;
    }

    private Game _game;
    private Vector2 _screenSize;
    private PackedScene _planetScene = GD.Load<PackedScene>("res://scenes/bodies/Planet.tscn");
    private List<Planet> _bigPlanets = new List<Planet>();
    // planet size
    private float _bigPlanetMinSizeScreenX = 0.25f;
    private float _bigPlanetMaxSizeScreenX = 0.35f;
    private float _smallPlanetMinSizeScreenX = 0.05f;
    private float _smallPlanetMaxSizeScreenX = 0.1f;
    // planet position
    private float _bigPlanetMinVerticalDistanceScreenX = 0.65f;
    private float _bigPlanetMaxVerticalDistanceScreenX = 0.75f;
    private float _smallPlanetMinDistanceBigPlanetRadius = 2.3f;
    private float _smallPlanetMaxDistanceBigPlanetRadius = 3f;
    // small planet apparition rate
    private float _smallPlanetApparitionRate = 0.4f;
    // planet movement
    private float _planetMinOrbitalSpeed = 0.5f;
    private float _planetMaxOrbitalSpeed = 1f;


    public override void _Ready()
    {
        _game = (Game)GetNode("/root/Game");
        _screenSize = GetViewport().GetVisibleRect().Size;
    }

    public override void _Process(float delta)
    {
        if (!_game.Started) {
            return;
        }

        if (_bigPlanets.Count == 0)
        {
            _GeneratePlanets();
        }
        else
        {
            // wait until the last planet has reached a certain point
            // before generating another one (for performance) 
            Planet lastPlanet = _bigPlanets[_bigPlanets.Count - 1];
            if (lastPlanet.GetGlobalTransformWithCanvas()[2].y > -_screenSize.y / 2)
            {
                _GeneratePlanets();
            }
        }
    }

    public void FadeAll() {
        // fade all planets
        foreach (Node2D body in GetTree().GetNodesInGroup("bodies"))
        {
            var planet = body as Planet;
            if (planet != null)
            {
                planet.Fade();
            }
        }

    }

    public void Remove(Planet planet)
    {
        if (_bigPlanets.Contains(planet)) {
            _bigPlanets.Remove(planet);
        }
    }

    private void _GeneratePlanets()
    {
        Planet bigPlanet = _GenerateBigPlanet();
        
        // randomize small planets apparition
        if ((float)GD.RandRange(0, 1) <= _smallPlanetApparitionRate)
        {
            _GenerateSmallPlanet(bigPlanet);
        }
    }

    private Planet _GenerateBigPlanet()
    {
        Planet planet = (Planet)_planetScene.Instance();
        planet.Radius = _GenerateBigPlanetRadius();
        planet.Position = _bigPlanets.Count == 0 ? _GenerateFirstBigPlanetPosition(planet.Radius) : _GenerateBigPlanetPosition(planet.Radius);
        _bigPlanets.Add(planet);
        AddChild(planet);
        EmitSignal("BigPlanetCreated", planet);
        return planet;
    }

    private float _GenerateBigPlanetRadius()
    {
        return (float)GD.RandRange(
            (_screenSize.x * _bigPlanetMinSizeScreenX) / 2,
            (_screenSize.x * _bigPlanetMaxSizeScreenX) / 2
        );
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
        Planet lastPlanet = _bigPlanets[_bigPlanets.Count - 1];
        float lastPlanetVerticalLimit = (lastPlanet.Position.y - lastPlanet.Radius);
        float minVerticalDistance = _screenSize.x * _bigPlanetMinVerticalDistanceScreenX;
        float maxVerticalDistance = _screenSize.x * _bigPlanetMaxVerticalDistanceScreenX;
        float verticalDistance = (float)GD.RandRange(minVerticalDistance, maxVerticalDistance);
        var y = lastPlanetVerticalLimit - verticalDistance;
        return new Vector2(x, y);
    }

    private Planet _GenerateSmallPlanet(Planet associatedBigPlanet)
    {
        Planet planet = (Planet)_planetScene.Instance();
        planet.Radius = _GenerateSmallPlanetRadius();
        planet.Position = _GenerateSmallPlanetPosition(associatedBigPlanet, planet.Radius);
        planet.BarycenterPosition = associatedBigPlanet.Position;
        planet.OrbitalSpeed = (float)GD.RandRange(_planetMinOrbitalSpeed, _planetMaxOrbitalSpeed);
        AddChild(planet);
        return planet;
    }

    private float _GenerateSmallPlanetRadius()
    {
        return (float)GD.RandRange(
            (_screenSize.x * _smallPlanetMinSizeScreenX) / 2,
            (_screenSize.x * _smallPlanetMaxSizeScreenX) / 2
        );
    }

    private Vector2 _GenerateSmallPlanetPosition(Planet associatedBigPlanet, float smallPlanetRadius)
    {
        Vector2 origin = associatedBigPlanet.Position;
        Vector2 position = new Vector2();
        Vector2 screenPosition = new Vector2();

        // from the origin, generate the position with a random direction and distance
        do {
            Vector2 direction = new Vector2(
                (float)GD.RandRange(-1, 1),
                (float)GD.RandRange(-1, 1)
            ).Normalized();
            float distance = (float)GD.RandRange(
                associatedBigPlanet.Radius * _smallPlanetMinDistanceBigPlanetRadius,
                associatedBigPlanet.Radius * _smallPlanetMaxDistanceBigPlanetRadius
            );
            position = (direction * distance) + origin;
            screenPosition = GetCanvasTransform().Xform(position);
            
            // if generated position is out of screen, try again
        } while (
            (screenPosition.x - smallPlanetRadius < 0) ||
            (screenPosition.x + smallPlanetRadius > _screenSize.x)
        );

        return position;
    }
}