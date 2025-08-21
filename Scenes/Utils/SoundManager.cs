using Godot;
using System;
using System.Collections.Generic;

public partial class SoundManager : Node
{
    public static SoundManager Instance { get; private set; }

    private static readonly int NB_CHANNELS = 4;

    private List<AudioStreamPlayer> _streamPlayers = new();

    public static readonly Dictionary<Sound, AudioStream> SFX_MAP = new()
    {
        {Sound.BOUNCE ,GD.Load<AudioStream>("res://Assets/sfx/bounce.wav") },
        {Sound.HURT ,GD.Load<AudioStream>("res://Assets/sfx/hurt.wav") },
        {Sound.PASS ,GD.Load<AudioStream>("res://Assets/sfx/pass.wav") },
        {Sound.POWERSHOT ,GD.Load<AudioStream>("res://Assets/sfx/power-shot.wav") },
        {Sound.SHOT ,GD.Load<AudioStream>("res://Assets/sfx/shoot.wav") },
        {Sound.TACKLING ,GD.Load<AudioStream>("res://Assets/sfx/tackle.wav") },
        {Sound.UI_NAV ,GD.Load<AudioStream>("res://Assets/sfx/ui-navigate.wav") },
        {Sound.UI_SELECT ,GD.Load<AudioStream>("res://Assets/sfx/ui-select.wav") },
        {Sound.WHISTLE ,GD.Load<AudioStream>("res://Assets/sfx/whistle.wav") },
    };

    public enum Sound
    {
        BOUNCE,
        HURT,
        PASS,
        POWERSHOT,
        SHOT,
        TACKLING,
        UI_NAV,
        UI_SELECT,
        WHISTLE
    }

    public override void _Ready()
    {
        Instance = this;
        for (int i = 0; i < NB_CHANNELS; i++)
        {
            var streamPlayer = new AudioStreamPlayer();
            _streamPlayers.Add(streamPlayer);
            AddChild(streamPlayer);
        }
    }

    public void Play(Sound sound)
    {
        var streamPlayer = FindFirstAvailablePlayer();
        if (streamPlayer != null)
        {
            streamPlayer.Stream = SFX_MAP[sound];
            streamPlayer.Play();
        }
    }

    private AudioStreamPlayer FindFirstAvailablePlayer()
    {
        foreach (AudioStreamPlayer streamPlayer in _streamPlayers)
        {
            if (!streamPlayer.Playing)
            {
                return streamPlayer;
            }
        }
        return null;
    }
}
