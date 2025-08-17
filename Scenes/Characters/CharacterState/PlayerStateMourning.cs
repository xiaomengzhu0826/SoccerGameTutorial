using Godot;
using System;

public partial class PlayerStateMourning : PlayerState
{
    public override void _EnterTree()
    {
        _animationPlayer.Play("mourn");
        _player.Velocity = Vector2.Zero;
        SignalManager.Instance.OnTeamReset += OnTeamReset;
    }

    public override void _ExitTree()
    {
        SignalManager.Instance.OnTeamReset -= OnTeamReset;
    }

    private void OnTeamReset()
    {
        EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.RESETING, PlayerStateData.Build().SetResetPosition(_player._KickoffPosition));
    }

}
