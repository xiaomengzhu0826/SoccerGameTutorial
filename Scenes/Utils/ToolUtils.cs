using Godot;
using System;
using System.Collections.Generic;

public class ToolUtils
{
    public static Dictionary<string, Texture2D> _FlagTextures = new();

    public static string GetScoreText(int[] score)
    {
        return $"{score[0]} - {score[1]}";
    }

    public static Texture2D GetTexture(string country)
    {
        if (!_FlagTextures.ContainsKey(country))
        {
            _FlagTextures.Add(country, GD.Load<Texture2D>($"res://Assets/art/ui/flags/flag-{country.ToLower()}.png"));
        }
        return _FlagTextures[country];
    }

    public static string GetTimeText(float timeLeft)
    {
        if (timeLeft < 0)
        {
            return "OVER TIME";
        }
        else
        {
            int minutes = (int)(timeLeft / 60.0);
            int seconds = (int)(timeLeft - minutes * 60);
            return $"{minutes:D2}:{seconds:D2}";
        }
    }

    public static string GetCurrentScoreInfo(List<string> countries, int[] score)
    {
        if (score[0] == score[1])
        {
            return $"TEAMS ARE TIED {score[0]} - {score[1]}";
        }
        else if (score[0] > score[1])
        {
            return $"{countries[0]} LEADS {score[0]} - {score[1]}";
        }
        else
        {
            return $"{countries[1]} LEADS {score[1]} - {score[0]}";
        }
    }
}
