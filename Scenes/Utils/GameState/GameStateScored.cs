using Godot;
using System;

public partial class GameStateScored : GameState
{
    private static readonly int DURATION_CELEBRATE = 3000;

    private float _timeSinceCelebrate = Time.GetTicksMsec();

    public override void _EnterTree()
    {
        _gameManager.IncreaseScore(_gameStateData._CountryScoredOn);
        _timeSinceCelebrate = Time.GetTicksMsec();
    }

    public override void _Process(double delta)
    {
        if (Time.GetTicksMsec() - _timeSinceCelebrate > DURATION_CELEBRATE)
        {
            TransitionState(GameManager.State.RESET,_gameStateData);
        }
    }
}
