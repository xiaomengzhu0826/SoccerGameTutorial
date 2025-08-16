using Godot;
using System;

public partial class GameStateGameOver : GameState
{
    public override void _EnterTree()
    {
        GD.Print("Game Over");
    }
}
