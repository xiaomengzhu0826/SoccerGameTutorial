using Godot;
using System;

public partial class PlayerStateShooting : PlayerState
{
    public override void _EnterTree()
    {
        _animationPlayer.Play("kick");
    }

    public override void OnAnimationCompelete()
    {
        if (_player._controlScheme == Player.ControlScheme.CPU)
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.RECOVERING,(PlayerStateData)null);
        }
        else
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.MOVING,(PlayerStateData)null);
        }
        ShootBall();
    }

    private void ShootBall()
    {
        SoundManager.Instance.Play(SoundManager.Sound.SHOT);
        _ball.Shoot(_playerStateData._ShotDirection * _playerStateData._ShotPower);
    }

}
