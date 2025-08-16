using Godot;
using System;

public partial class GameStateScored : GameState
{
    private static readonly int DURATION_CELEBRATE = 3000;

    private float _timeSinceCelebrate = Time.GetTicksMsec();

    public override void _EnterTree()
    {
        var indexCountryScoring = _gameStateData._CountryScoredOn == _gameManager._Countries[0] ? 1 : 0;
        GameManager.Instance._Score[indexCountryScoring] += 1;
        _timeSinceCelebrate = Time.GetTicksMsec();
    }

    public override void _Process(double delta)
    {
        if (Time.GetTicksMsec() - _timeSinceCelebrate > DURATION_CELEBRATE)
        {
            TransitionState(GameManager.State.RESET);
        }
    }
}
