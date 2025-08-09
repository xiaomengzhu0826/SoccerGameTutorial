using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class PlayerResource 
{
    [JsonProperty("name")]
    [Export] public string FullName { get; set; }

    [Export] public Player.SkinColor SkinColor { get; set; }
    [Export] public Player.Role Role { get; set; }
    [Export] public float Speed { get; set; }
    [Export] public float Power { get; set; }

    // public PlayerResource(string playerName, Player.SkinColor skinColor, Player.Role role, float speed, float power)
    // {
    //     FullName = playerName;
    //     SkinColor = skinColor;
    //     Role = role;
    //     Speed = speed;
    //     Power = power;
    // }
}

public class CountryData
{
    public string Country { get; set; }
    public List<PlayerResource> Players { get; set; }
}
