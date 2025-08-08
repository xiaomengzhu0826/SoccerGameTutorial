using Godot;
using System;

public partial class PlayerStateChestControl : PlayerState
{
    private static readonly float DURATION_CONTROL = 500.0f;

    private float _timeSinceControl = Time.GetTicksMsec();

    public override void _EnterTree()
    {
        _animationPlayer.Play("chest_control");
        _player.Velocity = Vector2.Zero;
        _timeSinceControl = Time.GetTicksMsec();
    }

    public override void _Process(double delta)
    {
        if (Time.GetTicksMsec() - _timeSinceControl > DURATION_CONTROL)
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.MOVING,(PlayerStateData)null);
        }
    }
}
