using Godot;
using System;

public partial class PlayerStateCelebrate : PlayerState
{
    private static readonly float CELEBRATE_HEIGHT = 1.0f;
    private static readonly float AIR_FRICTION = 35.0f;

    public override void _EnterTree()
    {
        Celebrate();
        SignalManager.Instance.OnTeamReset += OnTeamReset;
    }

    public override void _ExitTree()
    {
        SignalManager.Instance.OnTeamReset -= OnTeamReset;
    }

    public override void _Process(double delta)
    {
        if (_player._Height == 0)
        {
            Celebrate();
        }
        _player.Velocity = _player.Velocity.MoveToward(Vector2.Zero, (float)delta * AIR_FRICTION);
    }

    private void Celebrate()
    {
        _animationPlayer.Play("celebrate");
        _player._Height = 0.1f;
        _player._HeightVelocity = CELEBRATE_HEIGHT;
    }

    private void OnTeamReset()
    {
        EmitSignal(PlayerState.SignalName.OnStateTransitionRequest, (int)Player.State.RESETING, PlayerStateData.Build().SetResetPosition(_player._SpawnPosition));
    }

    
}
