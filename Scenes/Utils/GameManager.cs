using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    private static readonly int DURATION_GAME_SEC = 120;
    private static readonly int DURATION_IMPACT_PAUSE = 100;

    public float _TimeLeft;
    public int[] _Score = [0, 0];
    public string[] _PlayerSetup=["FRANCE", ""] ;

    private readonly GameStateFactory _gameStateFactory = new();
    private GameState _currentState;
    private float _timeSincePaused = Time.GetTicksMsec();

    public enum State
    {
        IN_PLAY,
        SCORED,
        RESET,
        KICKOFF,
        OVERTIME,
        GAMEOVER
    }

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
        _TimeLeft = DURATION_GAME_SEC;
        SignalManager.Instance.OnImpactReceived += OnImpactReceived;
        SwitchState(State.RESET);
    }

    public override void _ExitTree()
    {
        SignalManager.Instance.OnImpactReceived -= OnImpactReceived;
    }

    public override void _Process(double delta)
    {
        if (GetTree().Paused && Time.GetTicksMsec() - _timeSincePaused > DURATION_IMPACT_PAUSE)
        {
            GetTree().Paused = false;
        }
    }

    public void SwitchState(State state, GameStateData data = null)
    {
        if (_currentState != null)
        {
            _currentState.QueueFree();
        }
        _currentState = _gameStateFactory.GetFreshState(state);
        _currentState.Setup(this, data);
        _currentState.OnStateTransitionRequest += SwitchState;
        _currentState.Name = "GameStateMachine:" + state.ToString();
        CallDeferred("add_child", _currentState);
    }

    public bool IsCoop()
    {
        return _PlayerSetup[0] == _PlayerSetup[1];
    }

    public bool IsSinglePlayer()
    {
        return _PlayerSetup[1] == string.Empty;
    }

    public bool IsGameTied()
    {
        return _Score[0] == _Score[1];
    }

    public bool IsTimeUp()
    {
        return _TimeLeft <= 0;
    }

    public string GetWinnerCountry()
    {
        if (IsGameTied())
        {
            return null;
        }
        return _Score[0] > _Score[1] ? DataLoader.Instance._Countries[0] :DataLoader.Instance._Countries[1];
    }

    public void IncreaseScore(string countryScoreOn)
    {
        var indexCountryScoring = countryScoreOn == DataLoader.Instance._Countries[0] ? 1 : 0;
        _Score[indexCountryScoring] += 1;
        SignalManager.EmitOnScoreChanged();
    }

    public bool HasSomeoneScored()
    {
        return _Score[0] > 0 || _Score[1] > 0;
    }

    private void OnImpactReceived(Vector2 impactPosition, bool isHighImpact)
    {
        if (isHighImpact)
        {
            _timeSincePaused=Time.GetTicksMsec();
            GetTree().Paused = true;
        }
    }


}
