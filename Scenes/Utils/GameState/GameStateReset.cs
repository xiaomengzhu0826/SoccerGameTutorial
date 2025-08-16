using Godot;
using System;

public partial class GameStateReset : GameState
{
    public override void _EnterTree()
    {
        GD.Print("Reset");
    }
}
