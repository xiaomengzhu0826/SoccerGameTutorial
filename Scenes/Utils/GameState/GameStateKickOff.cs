using Godot;
using System;

public partial class GameStateKickOff : GameState
{
    public override void _EnterTree()
    {
        GD.Print("kickoff");
    }
    
}
