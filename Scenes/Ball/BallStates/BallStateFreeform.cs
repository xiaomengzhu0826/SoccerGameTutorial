using Godot;
using System;
using System.Diagnostics;

public partial class BallStateFreeform : BallState
{
    private static readonly float MAX_CAPTURE_HEIGHT = 25.0f;

    private float  _timeSinceFreeform=Time.GetTicksMsec();

    public override void _EnterTree()
    {
        _playerDetectionArea.BodyEntered += OnPlayerEnter;
        _timeSinceFreeform=Time.GetTicksMsec();
    }

    public override void _ExitTree()
    {
        _playerDetectionArea.BodyEntered -= OnPlayerEnter;
    }

    public override void _Process(double delta)
    {
        if (_ballStateData == null)
        {
            _ballStateData = BallStateData.Build().SetLockDuration(0);
        }
        _playerDetectionArea.Monitoring = Time.GetTicksMsec() - _timeSinceFreeform > _ballStateData.LockDuration;
        SetBallAnimationFromVelocity();
        float friction = _ball._height > 0 ? Ball.FRICTIONAIR : Ball.FRICTIONGROUND;
        _ball._velocity = _ball._velocity.MoveToward(Vector2.Zero, friction * (float)delta);
        ProcessGravity((float)delta, Ball.BOUNCINESS);
        MoveAndBounce((float)delta);
    }

    private void OnPlayerEnter(Node2D body)
    {
        Player player = (Player)body;
        if (player.CanCarryBall() && _ball._height<MAX_CAPTURE_HEIGHT)
        {
            _ball._carrier = player;
            player.ControlBall();
            EmitSignal(BallState.SignalName.OnStateTransitionRequest, (int)Ball.State.CARRIED,(BallStateData)null);
        }

    }

    public override bool CanAirInteract()
    {
        return true;
    }

}
