using Godot;
using System;

public partial class PlayerStateMoving : PlayerState
{
    public override void _Process(double delta)
    {
        if (_player._controlScheme == Player.ControlScheme.CPU)
        {
            _aiBehavior.ProcessAi();
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
        if (_player.Velocity != Vector2.Zero)
        {
            _teammateDetectionArea.Rotation = _player.Velocity.Angle();
        }

        if (_player.HasBall())
        {
            if (KeyUtils.IsActionJustPressed(_player._controlScheme, KeyUtils.Action.PASS))
            {
                EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.PASSING, (PlayerStateData)null);
            }
            else if (KeyUtils.IsActionJustPressed(_player._controlScheme, KeyUtils.Action.SHOOT))
            {
                EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.PREPPING_SHOT, (PlayerStateData)null);
            }
        }
        else if (_ball.CanAirInteract() && KeyUtils.IsActionJustPressed(_player._controlScheme, KeyUtils.Action.SHOOT))
        {
            if (_player.Velocity == Vector2.Zero)
            {
                if (_player.IsFacingTargetGoal())
                {
                    EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.VOLLEY_KICK, (PlayerStateData)null);
                }
                else
                {
                    EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.BICYCLE_KICK, (PlayerStateData)null);
                }
            }
            else
            {
                EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.HEADER, (PlayerStateData)null);
            }
        }


        if (_player.Velocity != Vector2.Zero && KeyUtils.IsActionJustPressed(_player._controlScheme, KeyUtils.Action.SHOOT))
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.TACKLING,(PlayerStateData)null);
        }
    }


}
