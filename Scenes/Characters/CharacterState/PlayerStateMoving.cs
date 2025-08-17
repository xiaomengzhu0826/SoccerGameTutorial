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
        _player.Velocity = direction * _player._Speed;
        if (_player.Velocity != Vector2.Zero)
        {
            _teammateDetectionArea.Rotation = _player.Velocity.Angle();
        }

        if (KeyUtils.IsActionJustPressed(_player._controlScheme, KeyUtils.Action.PASS))
        {
            if (_player.HasBall())
            {
                EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.PASSING, (PlayerStateData)null);
            }
            else if (CanTeammatePassBall())
            {
                _ball._carrier.GetPassRequest(_player);
            }
            else
            {
                _player.EmitSignal(nameof(Player.OnSwapRequest), _player);
            }
        }
        else if (KeyUtils.IsActionJustPressed(_player._controlScheme, KeyUtils.Action.SHOOT))
        {
            if (_player.HasBall())
            {
                EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.PREPPING_SHOT, (PlayerStateData)null);
            }
            else if (_ball.CanAirInteract())
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
            else if (_player.Velocity != Vector2.Zero)
            {
                EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.TACKLING, (PlayerStateData)null);
            }
        }     
    }

    public override bool CanCarryBall()
    {
        return _player._Role != Player.Role.GOALIE;
    }

    public bool CanTeammatePassBall()
    {
        return _ball._carrier != null && _ball._carrier._Country == _player._Country && _ball._carrier._controlScheme == Player.ControlScheme.CPU;
    }

    public override bool CanPass()
    {
        return true;
    }

}
