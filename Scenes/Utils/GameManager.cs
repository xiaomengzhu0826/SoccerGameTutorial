using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    private static readonly int DURATION_GAME_SEC = 120;

    public float _TimeLeft;
    public List<string> _Countries = new() { "FRANCE", "ENGLAND","ARGENTINA", "BRAZIL", "GERMANY", "ITALY", "SPAIN", "USA" };
    public List<int> _Score = new() { 0, 0 };

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

        SwitchState(State.IN_PLAY);
    }

    public void SwitchState(State state,GameStateData data=null)
    {
        if (_currentState != null)
        {
            _currentState.QueueFree();
        }
        _currentState = _gameStateFactory.GetFreshState(state);
        _currentState.Setup(this,data);
        _currentState.OnStateTransitionRequest += SwitchState;
        _currentState.Name = "GameStateMachine:" + state.ToString();
        CallDeferred("add_child", _currentState);
    }


}
