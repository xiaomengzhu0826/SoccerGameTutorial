using Godot;
using System;

public partial class PlayerStateTackling : PlayerState
{
    private const int DURATION_PRIOR_RECOVERY = 200;
    private const float GROUND_FRICTION = 250.0f;

    private float _timeFinishTackle;
    private bool _isTackleCompelelte;

    public override void _EnterTree()
    {
        _animationPlayer.Play("tackle");
        _tackleDamageEmitterArea.Monitoring = true;
    }
    
    public override void _ExitTree()
    {
         _tackleDamageEmitterArea.Monitoring = false;
    }

    public override void _Process(double delta)
    {
        if (!_isTackleCompelelte)
        {
            _player.Velocity = _player.Velocity.MoveToward(Vector2.Zero, (float)delta * GROUND_FRICTION);
            if (_player.Velocity == Vector2.Zero)
            {
                _isTackleCompelelte = true;
                _timeFinishTackle = Time.GetTicksMsec();
            }
        }
        else if (Time.GetTicksMsec() - _timeFinishTackle > DURATION_PRIOR_RECOVERY)
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.RECOVERING, (PlayerStateData)null);
        }
    }

}
