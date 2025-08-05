using Godot;
using System;

public partial class PlayerStateRecovering : PlayerState
{
    private const int DURATION_RECOVERY = 500;

    private float _timeStartRecovery = Time.GetTicksMsec();

    public override void _EnterTree()
    {
        _timeStartRecovery = Time.GetTicksMsec();
        _player.Velocity = Vector2.Zero;
        _animationPlayer.Play("recover");
    }

    public override void _Process(double delta)
    {
        if (Time.GetTicksMsec() - _timeStartRecovery > DURATION_RECOVERY)
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.MOVING,(PlayerStateData)null);
        }
    }

}
