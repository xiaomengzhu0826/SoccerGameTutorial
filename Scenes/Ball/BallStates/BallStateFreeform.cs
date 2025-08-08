using Godot;
using System;
using System.Diagnostics;

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

    public override void _Process(double delta)
    {
        SetBallAnimationFromVelocity();
        float friction = _ball._height > 0 ? Ball.FRICTIONAIR : Ball.FRICTIONGROUND;
        _ball._velocity = _ball._velocity.MoveToward(Vector2.Zero, friction * (float)delta);
        ProcessGravity((float)delta, Ball.BOUNCINESS);
        MoveAndBounce((float)delta);
    }

    private void OnPlayerEnter(Node2D body)
    {
        Player player = (Player)body;
        _ball._carrier = player;
        player.ControlBall();
        EmitSignal(BallState.SignalName.OnStateTransitionRequest, (int)Ball.State.CARRIED);
    }

    public override bool CanAirInteract()
    {
        return true;
    }

}
