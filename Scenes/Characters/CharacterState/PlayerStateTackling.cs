using Godot;
using System;

public partial class PlayerStateTackling : PlayerState
{
    private const int DURATION_TACKLE = 200;

    private float _timeStartTackle;

    public override void _EnterTree()
    {
        _animationPlayer.Play("tackle");
        _timeStartTackle = Time.GetTicksMsec();
    }

    public override void _Process(double delta)
    {
        if (Time.GetTicksMsec() - _timeStartTackle > DURATION_TACKLE)
        {
            EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.MOVING);
        }
    }

}
