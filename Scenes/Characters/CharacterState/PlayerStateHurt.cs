using Godot;
using System;

public partial class PlayerStateHurt : PlayerState
{
    private static readonly int DURATION_HURT = 1000;
    private static readonly float BALL_TUMBLE_SPEED = 100.0f;
    private static readonly float AIR_FRICTION = 35.0f;
    private static readonly float HURT_HEIGHT_VELOCITY = 1.0f;

    private float _timeStartHurt = Time.GetTicksMsec();

    public override void _EnterTree()
    {
        _animationPlayer.Play("hurt");

        _timeStartHurt = Time.GetTicksMsec();
        _player._HeightVelocity = HURT_HEIGHT_VELOCITY;
        _player._Height = 0.01f;
        if (_ball._carrier == _player)
        {
            _ball.Tumble(_playerStateData._HurtDirection * BALL_TUMBLE_SPEED);
            SoundManager.Instance.Play(SoundManager.Sound.HURT);
            SignalManager.EmitOnImpactReceived(_player.Position, false);
        }
    }

    public override void _Process(double delta)
    {
        if (Time.GetTicksMsec() - _timeStartHurt > DURATION_HURT)
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.RECOVERING, (PlayerStateData)null);
        }
        _player.Velocity = _player.Velocity.MoveToward(Vector2.Zero, (float)delta * AIR_FRICTION);
    }
}
