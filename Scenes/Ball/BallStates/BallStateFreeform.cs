using Godot;
using System;
using System.Diagnostics;

public partial class BallStateFreeform : BallState
{
    private readonly float BOUNCINESS = 0.8f;
    private readonly float FRICTION_AIR = 35.0f;
    private readonly float FREICTION_GROUND = 250.0f;

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
        float friction = _ball._height > 0 ? FRICTION_AIR : FREICTION_GROUND;
        _ball._velocity = _ball._velocity.MoveToward(Vector2.Zero, friction * (float)delta);
        ProcessGravity((float)delta,BOUNCINESS);
        _ball.MoveAndCollide(_ball._velocity * (float)delta);
    }

    private void OnPlayerEnter(Node2D body)
    {
        Player player = (Player)body;
        _ball._carrier = player;
        EmitSignal(BallState.SignalName.OnStateTransitionRequest, (int)Ball.State.CARRIED);
    }



}
