using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    private static readonly int DURATION_GAME_SEC = 120;

    public float _TimeLeft;
    public List<string> _Countries = new() { "FRANCE", "ENGLAND", "ARGENTINA", "BRAZIL", "GERMANY", "ITALY", "SPAIN", "USA" };
    public int[] _Score =[0,0] ;
    public List<string> _PlayerSetup = new() { "FRANCE", "" };

    private readonly GameStateFactory _gameStateFactory = new();
    private GameState _currentState;

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
        _TimeLeft = DURATION_GAME_SEC;

        SwitchState(State.RESET);
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


}
