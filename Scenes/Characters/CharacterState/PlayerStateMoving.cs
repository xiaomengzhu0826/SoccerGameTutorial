using Godot;
using System;

public partial class PlayerStateMoving : PlayerState
{
    public override void _Process(double delta)
    {
        if (_player._controlScheme == Player.ControlScheme.CPU)
        {

        }
        else
        {
            HandleHumanMovement();
        }
        _player.SetMovementAnimation();
        _player.SetHeading();
    }

    private void HandleHumanMovement()
    {
        var direction = KeyUtils.GetInputVector(_player._controlScheme);
        _player.Velocity = direction * _player._speed;

        if (_player.HasBall() && KeyUtils.IsActionJustPressed(_player._controlScheme, KeyUtils.Action.SHOOT))
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.PREPPING_SHOT);
        }

        // if (_player.Velocity != Vector2.Zero && KeyUtils.IsActionJustPressed(_player._controlScheme, KeyUtils.Action.SHOOT))
        // {
        //     EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.TACKLING);
        // }
    }
}
