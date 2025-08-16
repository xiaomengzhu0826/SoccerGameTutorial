using Godot;
using System;

public partial class GameState : Node
{
    [Signal] public delegate void OnStateTransitionRequestEventHandler(GameManager.State newState,GameStateData data);

    // public static void EmitOnStateTransitionRequest(GameManager.State newState)
    // {
    //     Instance.EmitSignal(SignalName.OnStateTransitionRequest,newState);
    // }

    protected GameManager _gameManager;
    protected GameStateData _gameStateData;

    public void Setup(GameManager contextManager, GameStateData contextData)
    {
        _gameManager = contextManager;
        _gameStateData = contextData;
    }

    public void TransitionState(GameManager.State newState,GameStateData data=null)
    {
        EmitSignal(SignalName.OnStateTransitionRequest,(int)newState,data);
    }
}
