using Godot;
using System;

public partial class GameStateGameOver : GameState
{
    public override void _EnterTree()
    {
        string countryWinner = GameManager.Instance.GetWinnerCountry();
        SignalManager.EmitOnGameOver(countryWinner);
    }
}
