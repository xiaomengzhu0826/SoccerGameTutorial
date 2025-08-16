using Godot;
using System;

public partial class GameStateData :Resource
{
    public string _CountryScoredOn;

    public static GameStateData Build()
    {
        return new GameStateData();
    }

    public GameStateData SetCountryScoredOn(string country)
    {
        _CountryScoredOn = country;
        return this;
    }
}
