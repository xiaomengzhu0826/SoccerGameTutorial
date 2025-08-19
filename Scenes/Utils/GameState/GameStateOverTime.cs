using Godot;
using System;

public partial class GameStateOverTime : GameState
{
    public override void _EnterTree()
    {
        SignalManager.Instance.OnTeamScored += OnTeamScored;
    }

    public override void _ExitTree()
    {
        SignalManager.Instance.OnTeamScored -= OnTeamScored;
    }

    private void OnTeamScored(string country)
    {
        _gameManager.IncreaseScore(country);
        TransitionState(GameManager.State.GAMEOVER);
    }

}
