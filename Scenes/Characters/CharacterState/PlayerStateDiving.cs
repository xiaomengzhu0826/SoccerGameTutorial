using Godot;
using System;

public partial class PlayerStateDiving : PlayerState
{
    private static readonly float DURATION_DIVE = 500;

    private float _timeStartDive = Time.GetTicksMsec();

    public override void _EnterTree()
    {
        var targetDive = new Vector2(_player._spawnPosition.X, _ball.Position.Y);
        var direction = _player.Position.DirectionTo(targetDive);
        if (direction.Y > 0)
        {
            _animationPlayer.Play("dive_down");
        }
        else
        {
            _animationPlayer.Play("dive_up");
        }
        _player.Velocity = direction * _player._speed;
        _timeStartDive = Time.GetTicksMsec();
    }

    public override void _Process(double delta)
    {
        if (Time.GetTicksMsec() - _timeStartDive > DURATION_DIVE)
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.RECOVERING,(PlayerStateData)null);
        }
    }
}
