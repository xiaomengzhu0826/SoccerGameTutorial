using Godot;
using System;

public partial class PlayerStateHeader : PlayerState
{
    private static readonly float HEIGHT_START = 0.1f;
    private static readonly float HEIGHT_VELOCITY = 1.5f;
    private static readonly float BOUNS_POWER = 1.3f;
    private static readonly float BALL_HEIGHT_MIN = 10f;
    private static readonly float BALL_HEIGHT_MAX = 30f;

    public override void _EnterTree()
    {
        _animationPlayer.Play("header");
        _player._Height = HEIGHT_START;
        _player._HeightVelocity = HEIGHT_VELOCITY;
        _ballDetectionArea.BodyEntered += OnBallEntered;
    }

    public override void _Process(double delta)
    {
        if (_player._Height == 0)
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.RECOVERING, (PlayerStateData)null);
        }
    }
    // public override void _ExitTree()
    // {
    //     _ballDetectionArea.BodyEntered -= OnBallEntered;
    // }

    private void OnBallEntered(Node2D body)
    {
        Ball contactBall = (Ball)body;
        if (contactBall.CanAirConnect(BALL_HEIGHT_MIN,BALL_HEIGHT_MAX))
        {
            SoundManager.Instance.Play(SoundManager.Sound.POWERSHOT);
            contactBall.Shoot(_player.Velocity.Normalized() * _player._Power * BOUNS_POWER);
        }
    }

}
