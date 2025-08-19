using Godot;
using System;

public partial class SignalManager : Node
{
    public static SignalManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    [Signal] public delegate void OnTeamScoredEventHandler(string country);

    public static void EmitOnTeamScored(string country)
    {
        Instance.EmitSignal(SignalName.OnTeamScored, country);
    }

    [Signal] public delegate void OnTeamResetEventHandler();

    public static void EmitOnTeamReset()
    {
        Instance.EmitSignal(SignalName.OnTeamReset);
    }

    [Signal] public delegate void OnKickoffReadyEventHandler();

    public static void EmitOnKickoffReady()
    {
        Instance.EmitSignal(SignalName.OnKickoffReady);
    }

    [Signal] public delegate void OnKickoffStartedEventHandler();

    public static void EmitOnKickoffStarted()
    {
        Instance.EmitSignal(SignalName.OnKickoffStarted);
    }

    [Signal] public delegate void OnBallPossessedEventHandler(string name);

    public static void EmitOnBallPossessed(string name)
    {
        Instance.EmitSignal(SignalName.OnBallPossessed, name);
    }

    [Signal] public delegate void OnBallReleasedEventHandler();

    public static void EmitOnBallReleased()
    {
        Instance.EmitSignal(SignalName.OnBallReleased);
    }
    
    [Signal] public delegate void OnScoreChangedEventHandler();
    
    public static void EmitOnScoreChanged()
    {
        Instance.EmitSignal(SignalName.OnScoreChanged);
    }
    

}
