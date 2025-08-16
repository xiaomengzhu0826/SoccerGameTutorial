using Godot;
using System;

public partial class GameStateInPlay : GameState
{
    public override void _EnterTree()
    {
        SignalManager.Instance.OnTeamScored += OnTeamScored;
    }

    public override void _ExitTree()
    {
         SignalManager.Instance.OnTeamScored -= OnTeamScored;
    }

    public override void _Process(double delta)
    {
        _gameManager._TimeLeft -= (float)delta;
        if (_gameManager._TimeLeft <= 0)
        {
            if (_gameManager._Score[0] == _gameManager._Score[1])
            {
                TransitionState(GameManager.State.OVERTIME);
            }
            else
            {
                TransitionState(GameManager.State.GAMEOVER);
            }
        }
    }

    private void OnTeamScored(string country)
    {
        TransitionState(GameManager.State.SCORED,GameStateData.Build().SetCountryScoredOn(country));
    }
}
