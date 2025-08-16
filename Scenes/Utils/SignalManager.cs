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
        Instance.EmitSignal(SignalName.OnTeamScored,country);
    }
}
