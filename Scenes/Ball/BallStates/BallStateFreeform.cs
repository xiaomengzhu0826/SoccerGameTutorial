using Godot;
using System;

public partial class BallStateFreeform : BallState
{
    public override void _EnterTree()
    {
        _playerDetectionArea.BodyEntered += OnPlayerEnter;
    }

    public override void _ExitTree()
    {
         _playerDetectionArea.BodyEntered -= OnPlayerEnter;
    }

    private void OnPlayerEnter(Node2D body)
    {
        Player player = (Player)body;
        _ball._carrier = player;
        EmitSignal(BallState.SignalName.OnStateTransitionRequest,(int)Ball.State.CARRIED);
    }

}
